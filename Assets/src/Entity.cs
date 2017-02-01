using UnityEngine;
using System.Collections.Generic;

public enum EntityType { RED, BLUE, ALL, NONE = -1 }

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

    public static GameObject cube = null;
    static Material red = null, blue = null;
    static Color select = new Color(0.2f, 0.2f, 0.2f, 0.0f);

    public GameObject obj = null;
    public GridPos gridPos = null;
    public List<Entity> friendList = new List<Entity>();
    public List<Entity> enemyList = new List<Entity>();

    AI ai = null;
    Vector3 targetPos;

    public EntityData GetData()
    {
        return crtData;
    }

    public static void StaticInit()
    {
        red = new Material(Shader.Find("Standard"));
        red.color = Color.red;
        blue = new Material(Shader.Find("Standard"));
        blue.color = Color.blue;
    }

    public Entity(EntityType t, float x, float y, string name)
    {
        Renderer r = cube.GetComponent<Renderer>();
        switch(t)
        {
            case EntityType.RED:
                r.material = red;
                break;
            case EntityType.BLUE:
                r.material = blue;
                break;
        }
        obj = (GameObject)GameObject.Instantiate(cube, new Vector3(x, 0, y), Quaternion.identity);
        obj.name = name;
        ai = obj.GetComponent<AI>();
        ai.Init(this, name);

        data.type = t;
        data.name = name;
        crtData.Copy(data);
    }

    public void Picked()
    {
        obj.GetComponent<Renderer>().material.color += select;
    }

    public void Unpicked()
    {
        obj.GetComponent<Renderer>().material.color -= select;
    }

    public void Update()
    {
        friendList.Clear();
        enemyList.Clear();
        Vector2 pos = new Vector2(obj.transform.position.x, obj.transform.position.z);
        List<GridPos> list = World.GetInstance().GetGrids(pos.x, pos.y, crtData.range);
        foreach (GridPos gridPos in list)
        {
            for (int i = 0; i < (int)EntityType.ALL; i++)
            {
                List<Entity> entityList = World.GetInstance().GetGrid(gridPos).entityList[i];
                foreach (Entity entity in entityList)
                {
                    Vector2 pos2 = new Vector2(entity.obj.transform.position.x, entity.obj.transform.position.z);
                    if ((pos2 - pos).magnitude >= crtData.range)
                        continue;
                    if ((int)crtData.type == i)
                        friendList.Add(entity);
                    else
                        enemyList.Add(entity);
                }
            }
        }

    }

    public void UpdateGrid()
    {
        Vector2 pos = new Vector2(obj.transform.position.x, obj.transform.position.z);
        GridPos grid_pos = World.GetInstance().PosToGridPos(pos);
        if(gridPos.x != grid_pos.x || gridPos.y != grid_pos.y)
        {
            World.GetInstance().Remove(this, grid_pos);
            World.GetInstance().Add(this);
        }
    }

public void FindTarget()
    {
        float gridSize = World.GetInstance().gridSize;
        float worldSize = World.GetInstance().worldSize * 0.5f;
        targetPos =  obj.transform.position + new Vector3((Random.value - 0.5f) * gridSize, 0.0f, (Random.value - 0.5f) * gridSize);
        if (targetPos.x > worldSize)
        {
            targetPos.x -= gridSize;
        } 
        else if(targetPos.x < -worldSize)
        {
            targetPos.x += gridSize;
        }
        if (targetPos.z > worldSize)
        {
            targetPos.z -= gridSize;
        }
        else if (targetPos.z < -worldSize)
        {
            targetPos.z += gridSize;
        }
    }

    public void StartIdle()
    {
    }

    public void Idling()
    {
        FindTarget();
        ai.ToState(AIState.MOVE);
    }

    public void EndIdle()
    {
    }

    public void StartMove()
    {
    }

    public void Moving()
    {
        Vector3 d = targetPos - obj.transform.position;
        if (d.sqrMagnitude  > crtData.speed * crtData.speed)
        {
            obj.transform.position += d.normalized * crtData.speed;
        }
        else
        {
            obj.transform.position = targetPos;
            ai.ToState(AIState.IDLE);
        }
    }

    public void EndMove()
    {
    }
}