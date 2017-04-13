using UnityEngine;
using System.Collections.Generic;

class GridData
{
    public int[] _risk = new int[(int)EntityType.ALL];
    public int _resource = 100;
}

public class Grid
{
    bool _dirty = false;
    public Vector2 _centerPos;
    public List<Entity>[] _entityList = new List<Entity>[(int)EntityType.ALL];

    public Grid()
    {
        for (int i = 0; i < (int)EntityType.ALL; i++)
        {
            _entityList[i] = new List<Entity>();
        }
    }
    
    public void AddEntity(Entity entity)
    {
        _entityList[(int)entity.GetCrtData()._type].Add(entity);
        _dirty = true;
    }

    public void RemoveEntity(Entity entity)
    {
        _entityList[(int)entity.GetCrtData()._type].Remove(entity);
        _dirty = true;
    }

    public int GetEntityNum(EntityType type = EntityType.ALL)
    {
        if (type != EntityType.ALL)
            return _entityList[(int)type].Count;

        int count = 0;
        for(int i = 0;  i < (int)EntityType.ALL; i++)
        {
            count += _entityList[i].Count;
        }
        return count;
    }

    public int GetEnemyNum(int type)
    {
        int count = 0;
        for (int i = 0; i < (int)EntityType.ALL; i++)
        {
            if (i == type)
                continue;
            count += _entityList[i].Count;
        }
        return count;
    }
};

public class GridPos
{
    static int _gridNum = 0;
    public GridPos(int xx = 0, int yy = 0)
    {
        x = xx;
        y = yy;
        if (x < 0)
            x = 0;
        if (x >= _gridNum)
            x = _gridNum - 1;
        if (y < 0)
            y = 0;
        if (y >= _gridNum)
            y = _gridNum - 1;
    }

    public static void SetGridNum(int num)
    {
        _gridNum = num;
    }

    public int x, y;
}

public class World
{
    public float _worldSize = 0.0f;
    public float _gridSize = 0.0f;
    public int _gridNum = 0;
    public Grid[,] _grid = null;

    public static World GetInstance() { return Singleton<World>.GetInstance(); }

    public void Add(Entity entity)
    {
        entity._gridPos = PosToGridPos(entity._obj.transform.position.x, entity._obj.transform.position.z);
        _grid[entity._gridPos.x, entity._gridPos.y].AddEntity(entity);
    }

    public void Remove(Entity entity, GridPos gridPos = null)
    {
        if (gridPos == null)
        {
            gridPos = entity._gridPos;
        }

        _grid[gridPos.x, gridPos.y].RemoveEntity(entity);
    }

    public void CreateWorld(float worldSize, float gridSize)
    {
        _gridSize = gridSize;
        _gridNum = (int)(worldSize / gridSize + 1.0f - Utility.MIN_FLOAT);
        _worldSize = _gridSize * _gridNum;
        GridPos.SetGridNum(_gridNum);
        _grid = new Grid[_gridNum, _gridNum];
        for (int i = 0; i < _gridNum; i++)
        {
            for (int j = 0; j < _gridNum; j++)
            {
                Grid g = new Grid();
                g._centerPos = GridPosToCenterPos(i, j);
                _grid[i, j] = g;
            }
        }
    }

    public GridPos PosToGridPos(float x, float y)
    {
        return new GridPos((int)((x + _worldSize * 0.5f) / _gridSize), (int)((y + _worldSize * 0.5f) / _gridSize));
    }

    public GridPos PosToGridPos(Vector2 pos)
    {
        return PosToGridPos(pos.x, pos.y);
    }

    public Vector2 GridPosToCenterPos(int gx, int gy)
    {
        return new Vector2((gx + 0.5f) * _gridSize - _worldSize * 0.5f, (gy + 0.5f) * _gridSize - _worldSize * 0.5f);
    }

    public Vector2 GetGridCenter(int gx, int gy)
    {
        return _grid[gx, gy]._centerPos;
    }

    public Vector2 GetGridCenter(GridPos gridPos)
    {
        return GetGridCenter(gridPos.x, gridPos.y);
    }

    public Grid GetGrid(int x, int y)
    {
        return _grid[x, y];
    }

    public Grid GetGrid(GridPos pos)
    {
        return GetGrid(pos.x, pos.y);
    }

    public List<GridPos> GetGrids(float x, float y, float range)
    {
        List<GridPos> objList = new List<GridPos>();
        GridPos gridPos1 = PosToGridPos(x - range, y - range);
        GridPos gridPos2 = PosToGridPos(x + range, y + range);

        Vector2 pos = new Vector2(x, y);

        for (int i = gridPos1.x; i <= gridPos2.x; i++)
        {
            for (int j = gridPos1.y; j <= gridPos2.y; j++)
            {
                Vector2 v = GetGridCenter(i, j);
                float len = _gridSize * 0.71f /* 2.0f ^ 0.5f * 0.5f */ + range;
                if ((v - pos).magnitude < len)
                {
                    GridPos gridPos = new GridPos(i, j);
                    objList.Add(gridPos);
                }
            }
        }

        return objList;
    }
}
