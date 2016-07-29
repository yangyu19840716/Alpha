using UnityEngine;
using System.Collections.Generic;

public class Entity {
    public enum Type { RED, BLUE, ALL, NONE }
    public enum State { IDLE, MOVE, NONE };
    public static GameObject red = null, blue = null;
    public static Material selectMat = null;

    public GameObject obj = null;
    public List<Entity> friendList = new List<Entity>();
    public List<Entity> enemyList = new List<Entity>();
    public Type type = Type.NONE;
    public GridPos gridPos = null;

    AI ai = null;
    Renderer render = null;
    Material origMat = null;
    int value = 0;
    int hp = 0;
    int maxHP = 100;
    int maxEnergy = 100;
    int energy = 0;
    int rank = 0;
    int resource = 0;
    int balance = 0; // 1 ～ 5 激进， -1 ～ -5 保守
    float range = 10.0f;
    float attackRange = 1.0f;
    float risk = 0.0f;
    float revenue = 0.0f;
    float speed = 0.001f;
    bool dying = false;
    bool attacked = false;
    Vector3 targetPos = new Vector3();
    Entity targetHunt = null;
    SceneManager sceneMgr = null;
    State state = State.IDLE;
    delegate bool deleAction();
    deleAction crtAction = null;
    public Entity(Type t, float x, float y, string name)
    {
        targetPos = new Vector3(x, 0, y);
        GameObject o = red;
        if (t == Type.BLUE)
            o = blue;
        obj = (GameObject)GameObject.Instantiate(o, targetPos, Quaternion.identity);
        render = obj.GetComponent<Renderer>();
        origMat = render.material;
        type = t;
        ai = obj.GetComponent<AI>();
        ai.owner = this;
        obj.name = name;
        sceneMgr = SceneManager.GetInstance();
    }

    public void Init()
    {
        hp = maxHP;
        energy = maxEnergy;

        Vector2 pos = new Vector2(obj.transform.position.x, obj.transform.position.z);
        // gridPos = Field.PosToGridPos(pos.x, pos.y);
        List<GridPos> list = Field.GetGrids(pos.x, pos.y, range);
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < (int)Type.ALL; j++)
            {
                List<Entity> entityList = Field.GetGrid(list[i]).entityList[j];
                for (int k = 0; k < entityList.Count; k++)
                {
                    Entity entity = entityList[k];
                    Vector2 pos2 = new Vector2(entity.obj.transform.position.x, entity.obj.transform.position.z);
                    if ((pos2 - pos).magnitude >= range)
                        continue;
                    if ((int)type == j)
                        friendList.Add(entity);
                    else
                        enemyList.Add(entity);
                }
            }
        }
    }

    public void ChooseAction()
    {
        if (IsInDanger())
            crtAction = Move;
    }

    public void Update()
    {
        if( IsInDanger() || gridPos != null && Field.GetGrid(gridPos).GetEntityNum() > GameConst.TOO_MANNY_ENTITY_IN_GRID)
        {
            float minRisk = 99999999;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int x = gridPos.x + i;
                    int y = gridPos.y + j;
                    if (x >= GridPos.gridNum || y >= GridPos.gridNum || x < 0 || y < 0)
                        continue;
                    int r = Risk(x, y);
                    Vector2 pos = Field.GetGridCenter(x, y);
                    float d = (pos - new Vector2(obj.transform.position.x, obj.transform.position.z)).magnitude;
                    float v = r * d;
                    if (r == 0)
                        v = -d;
                    if (minRisk > v)
                    {
                        minRisk = v;
                        targetPos.x = pos.x;
                        targetPos.z = pos.y;
                    }
                }
            }
            Move();
        }
        else
        {
        }
    }

    int Risk(GridPos pos)
    {
        return Risk(pos.x, pos.y);
    }

    int Risk(int x, int y)
    {
        Grid g = Field.GetGrid(x, y);
        return g.risk[(int)type];
    }

    bool IsInDanger()
    {
        int count = 0;
        float hp_percent = (float)hp / maxHP;
        if (enemyList.Count > GameConst.ENEMY_DANGER_NUM)
            return true;
        for (int i = 0; i < enemyList.Count; i++)
        {
            Entity enemy = enemyList[i];
            if (enemy.targetHunt == this)
            {
                if (enemy.GetRank() - rank >= GameConst.RANK_DANGER_LEVEL * hp_percent)
                    return true;

                count++;
                if (count >= GameConst.HUNT_TARGET_DANGER_NUM * hp_percent)
                    return true;
            }
        }

        return false;
    }

    bool IsNeedHelp(Entity entity)
    {
        if (entity.IsInDanger() && entity.attacked)
            return true;

        return false;
    }

    public int GetRank()
    {
        return rank;
    }

    public float GetRange()
    {
        return range;
    }

    public void Picked()
    {
        selectMat.color = origMat.color + new Color(0.3f, 0.3f, 0.3f, 1.0f);
        render.material = selectMat;
    }

    public void Unpicked()
    {
        render.material = origMat;
    }

    bool ExpendEnergy(int v)
    {
        if (energy - v > 0)
        {
            energy -= v;
            if (energy > maxEnergy)
                energy = maxEnergy;
        }
        else
        {
            if (ExpendHP(1))
                energy += (int)(GameConst.HP_ENERGY_RATE * maxEnergy);
            else
                return false;
        }
        return true;
    }

    bool ExpendHP(int v)
    {
        if (hp - v > 0)
        {
            hp -= v;
            if (hp > maxHP)
                hp = maxHP;
        }
        else
            return false;
        return true;
    }

    void Dying()
    {
        hp = 0;
        energy = 0;
        dying = true;
        sceneMgr.EntityDie(this);
    }

    void Attacked(Entity attacker)
    {
        attacked = true;
        int d = attacker.GetRank() - rank + GameConst.RANK_DANGER_LEVEL;
        if (d <= 0)
            return;

        if (!ExpendHP(d * d))
            Dying();
    }

    bool Rest()
    {
        if (hp < maxHP && energy > maxEnergy * GameConst.HP_RECOVER_PERCENT)
        {
            int d = (int)(GameConst.HP_RECOVER_RATE * maxHP);
            d = d == 0 ? -1 : -d;
            ExpendHP(d);
        }

        if (energy < maxEnergy)
        {
            int d = (int)(GameConst.ENERGY_RECOVER_RATE * maxEnergy);
            d = d == 0 ? -1 : -d;
            ExpendEnergy(d);
        }

        return true;
    }

    bool Move()
    {
        Vector3 d = targetPos - obj.transform.position;
        if (d.magnitude < 0.0001f)
            return false;

        if (!ExpendEnergy(GameConst.MAVE_ENERGY))
            return false;

        if (d.magnitude > speed)
        {
            obj.transform.position += d.normalized * speed;
        }
        else
        {
            obj.transform.position = targetPos;
        }

        GridPos grid = Field.PosToGridPos(new Vector2(obj.transform.position.x, obj.transform.position.z));
        if (gridPos.x != grid.x || gridPos.y != grid.y)
        {
            Field.RemoveFromField(this);
            Field.AddToField(this);
        }
        return true;
    }

    bool Hunt()
    {
        if (targetHunt == null || !ExpendEnergy(GameConst.HUNT_ENERGY))
            return false;

        targetPos = targetHunt.obj.transform.position;
        if ((targetPos - obj.transform.position).magnitude < attackRange)
            targetHunt.Attacked(this);
        else
            Move();
        return true;
    }

    bool Farm()
    {
        if (!ExpendEnergy(GameConst.FARM_ENERGY))
            return false;
        Grid grid = Field.GetGrid(gridPos);
        int res = grid.ExpendResource(GameConst.FARM_RESOURCE);
        resource += res;
        return res > 0;
    }
}
