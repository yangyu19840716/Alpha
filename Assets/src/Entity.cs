using UnityEngine;
using System.Collections.Generic;

public enum EntityType { RED, BLUE, ALL, NONE = -1 }


public class EntityData
{
    public string _name = "";
    public int _value = 0;
    public int _hp = 100;
    public int _energy = 0;
    public int _resource = 0;
    public int _balance = 0; // 1 ～ 5 激进， -1 ～ -5 保守
    public float _range = 10.0f;
    public float _attack = 1.0f;
    public float _revenue = 0.0f;
    public float _speed = 0.1f;
    public float _attackRange = 1.0f;
    public EntityType _type = EntityType.NONE;

    public void Copy(EntityData data)
    {
        _name = data._name;
        _value = data._value;
        _hp = data._hp;
        _energy = data._energy;
        _resource = data._resource;
        _balance = data._balance;
        _range = data._range;
        _attack = data._attack;
        _revenue = data._revenue;
        _speed = data._speed;
        _attackRange = data._attackRange;
        _type = data._type;
    }
}

public class Entity
{
    public static GameObject _cube = Resources.Load("prefab/cube") as GameObject;
    static Color _select = new Color(0.2f, 0.2f, 0.2f, 0.0f);

    EntityData _data = new EntityData();
    EntityData _crtData = new EntityData();

    public GameObject _obj = null;
    public GridPos _gridPos = null;
    public List<Entity> _friendList = new List<Entity>();
    public List<Entity> _enemyList = new List<Entity>();

    AI _ai = null;

    public EntityData GetData()
    {
        return _data;
    }

    public EntityData GetCrtData()
    {
        return _crtData;
    }

    public Entity(EntityType t, float x, float y, string name)
    {
        _obj = (GameObject)GameObject.Instantiate(_cube, new Vector3(x, 0, y), Quaternion.identity);
        _obj.name = name;

        Renderer r = _obj.GetComponent<Renderer>();
        switch(t)
        {
            case EntityType.RED:
                r.material.color = Color.red;
                break;
            case EntityType.BLUE:
                r.material.color = Color.blue;
                break;
        }

        _data._type = t;
        _data._name = name;
        _crtData.Copy(_data);

        _ai = _obj.GetComponent<AI>();
        _ai.Init(this, name);
    }

    public void Picked()
    {
        _obj.GetComponent<Renderer>().material.color += _select;
        DebugModule.ShowCircle(this, _crtData._range * 2.0f);
    }

    public void Unpicked()
    {
        _obj.GetComponent<Renderer>().material.color -= _select;
        DebugModule.HideCircle();
    }

    public void Update()
    {
        _friendList.Clear();
        _enemyList.Clear();
        Vector2 pos = new Vector2(_obj.transform.position.x, _obj.transform.position.z);
        List<GridPos> list = World.GetInstance().GetGrids(pos.x, pos.y, _crtData._range);
        foreach (GridPos gridPos in list)
        {
            for (int i = 0; i < (int)EntityType.ALL; i++)
            {
                List<Entity> entityList = World.GetInstance().GetGrid(gridPos)._entityList[i];
                foreach (Entity entity in entityList)
                {
                    Vector2 pos2 = new Vector2(entity._obj.transform.position.x, entity._obj.transform.position.z);
                    if ((pos2 - pos).magnitude >= _crtData._range)
                        continue;
                    if ((int)_crtData._type == i)
                        _friendList.Add(entity);
                    else
                        _enemyList.Add(entity);
                }
            }
        }
    }

    public void UpdateGrid()
    {
        Vector2 pos = new Vector2(_obj.transform.position.x, _obj.transform.position.z);
        GridPos gridPos = World.GetInstance().PosToGridPos(pos);
        if(_gridPos.x != gridPos.x || _gridPos.y != gridPos.y)
        {
            World.GetInstance().Remove(this, gridPos);
            World.GetInstance().Add(this);
        }
    }

    public void AttackAni()
    {
            
    }

    public void AttackedAni()
    {

    }
}