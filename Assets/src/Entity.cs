using UnityEngine;
using System.Collections.Generic;

public enum EntityType { RED, BLUE, ALL, NONE = -1 }
public enum EntityState { IDLE, MOVE, ALL, NONE = -1 }

public class EntityData
{
    public string name = "";
    public int value = 0;
    public int hp = 100;
    public int energy = 0;
    public int resource = 0;
    public int balance = 0; // 1 ～ 5 激进， -1 ～ -5 保守
    public float range = 10.0f;
    public float attack = 1.0f;
    public float revenue = 0.0f;
    public float speed = 1.0f;
    public EntityType type = EntityType.NONE;

    public void Copy(EntityData data)
    {
        name = data.name;
        value = data.value;
        hp = data.hp;
        energy = data.energy;
        resource = data.resource;
        balance = data.balance;
        range = data.range;
        attack = data.attack;
        revenue = data.revenue;
        speed = data.speed;
        type = data.type;
    }
}

public class Entity
{
    EntityData data = new EntityData();
    EntityData crtData = new EntityData();

    public static GameObject red = null, blue = null;
    public static Material selectMat = null;

    public GameObject obj = null;
    public GridPos gridPos = null;
    public List<Entity> friendList = new List<Entity>();
    public List<Entity> enemyList = new List<Entity>();

    AI ai = null;
    Renderer render = null;
    Material origMat = null;
    Vector3 targetPos = new Vector3();
    Entity targetHunt = null;

    public EntityData GetData()
    {
        return crtData;
    }

    public Entity(EntityType t, float x, float y, string name)
    {
        targetPos = new Vector3(x, 0, y);
        GameObject o = red;
        if (t == EntityType.BLUE)
            o = blue;
        obj = (GameObject)GameObject.Instantiate(o, targetPos, Quaternion.identity);
        obj.name = name;
        render = obj.GetComponent<Renderer>();
        origMat = render.material;
        ai = obj.GetComponent<AI>();
        ai.owner = this;
        data.type = t;
        data.name = name;
        crtData.Copy(data);
    }

    public void Init()
    {
        Vector2 pos = new Vector2(obj.transform.position.x, obj.transform.position.z);
        // gridPos = Field.PosToGridPos(pos.x, pos.y);
        List<GridPos> list = World.GetInstacne().GetGrids(pos.x, pos.y, crtData.range);
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < (int)EntityType.ALL; j++)
            {
                List<Entity> entityList = World.GetInstacne().GetGrid(list[i]).entityList[j];
                for (int k = 0; k < entityList.Count; k++)
                {
                    Entity entity = entityList[k];
                    Vector2 pos2 = new Vector2(entity.obj.transform.position.x, entity.obj.transform.position.z);
                    if ((pos2 - pos).magnitude >= crtData.range)
                        continue;
                    if ((int)crtData.type == j)
                        friendList.Add(entity);
                    else
                        enemyList.Add(entity);
                }
            }
        }
    }

    public int GetRank()
    {
        return 0;
    }

    public void Update()
    {
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
}