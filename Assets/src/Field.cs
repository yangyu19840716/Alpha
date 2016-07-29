using UnityEngine;
using System.Collections.Generic;

public class Grid
{
    public List<Entity>[] entityList = new List<Entity>[(int)Entity.Type.ALL];
    public int[] risk = new int[(int)Entity.Type.ALL];
    public int resource = 100;
    public bool dirty = false;
    public Vector2 centerPos;

    public Grid()
    {
        for (int i = 0; i < (int)Entity.Type.ALL; i++)
        {
            entityList[i] = new List<Entity>();
        }
    }

    public int ExpendResource(int v)
    {
        if (resource > v)
        {
            resource -= v;
            return v;
        }

        int tmp = resource;
        resource = 0;
        return tmp;
    }
    
    public void AddEntity(Entity entity)
    {
        entityList[(int)entity.type].Add(entity);
        dirty = true;
    }

    public void RemoveEntity(Entity entity)
    {
        entityList[(int)entity.type].Remove(entity);
        dirty = true;
    }

    public int GetEntityNum(int type = -1)
    {
        if (type >= 0)
            return entityList[type].Count;

        int count = 0;
        for(int i = 0;  i < (int)Entity.Type.ALL; i++)
        {
            count += entityList[i].Count;
        }
        return count;
    }

    public int GetEnemyNum(int type)
    {
        int count = 0;
        for (int i = 0; i < (int)Entity.Type.ALL; i++)
        {
            if (i == type)
                continue;
            count += entityList[i].Count;
        }
        return count;
    }

    public void UpdateRisk()
    {
        int count = (int)Entity.Type.ALL;
        int[] r = new int[count];
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < entityList[i].Count; j++)
            {
                Entity e = entityList[i][j];
                r[i] += e.GetRank() * GameConst.RANK_SCALE;
            }
        }

        for (int i = 0; i < count; i++)
        {
            risk[i] = 0;
            for (int j = 0; j < count; j++)
            {
                if (j == i)
                    continue;
                risk[i] += r[j];
            }
        }

        dirty = false;
    }
};

public class GridPos
{
    static public int gridNum = 0;
    public GridPos(int xx = 0, int yy = 0)
    {
        x = xx;
        y = yy;
        if (x < 0)
            x = 0;
        if (x >= gridNum)
            x = gridNum - 1;
        if (y < 0)
            y = 0;
        if (y >= gridNum)
            y = gridNum - 1;
    }

    public int x, y;
}


public class Field {
    static Field instance = null;

    public float fieldSize = 0.0f;
    public float gridSize = 0.0f;
    public Grid[,] grid = null;

    Field() { }

    public void Update()
    {
        for (int i = 0; i < GridPos.gridNum; i++)
        {
            for (int j = 0; j < GridPos.gridNum; j++)
            {
                Grid g = grid[i, j];
                if (!g.dirty)
                    continue;

                g.UpdateRisk();
            }
        }
    }

    static public void AddToField(Entity entity)
    {
        entity.gridPos = PosToGridPos(entity.obj.transform.position.x, entity.obj.transform.position.z);
        instance.grid[entity.gridPos.x, entity.gridPos.y].AddEntity(entity);
    }

    static public void RemoveFromField(Entity entity)
    {
        instance.grid[entity.gridPos.x, entity.gridPos.y].RemoveEntity(entity);
    }

    static public Field CreateField(float field_size, float grid_size)
    {
        if (instance != null)
            return instance;

        instance = new Field();
        instance.fieldSize = field_size;
        instance.gridSize = grid_size;
        GridPos.gridNum = (int)(instance.fieldSize / instance.gridSize + 0.9999);
        instance.grid = new Grid[GridPos.gridNum, GridPos.gridNum];
        for (int i = 0; i < GridPos.gridNum; i++)
        {
            for (int j = 0; j < GridPos.gridNum; j++)
            {
                Grid g = new Grid();
                g.centerPos = GridPosToCenterPos(i, j);
                instance.grid[i, j] = g;
            }
        }
        return instance;
    }

    static public GridPos PosToGridPos(float x, float y)
    {
        return new GridPos((int)((x + instance.fieldSize / 2) / instance.gridSize), (int)((y + instance.fieldSize / 2) / instance.gridSize));
    }

    static public GridPos PosToGridPos(Vector2 pos)
    {
        return PosToGridPos(pos.x, pos.y);
    }

    static public Vector2 GridPosToCenterPos(int gx, int gy)
    {
        return new Vector2((gx + 0.5f) * instance.gridSize - instance.fieldSize / 2, (gy + 0.5f) * instance.gridSize - instance.fieldSize / 2);
    }

    static public Vector2 GetGridCenter(int gx, int gy)
    {
        return instance.grid[gx, gy].centerPos;
    }

    static public Vector2 GetGridCenter(GridPos gridPos)
    {
        return GetGridCenter(gridPos.x, gridPos.y);
    }

    static public Grid GetGrid(int x, int y)
    {
        return instance.grid[x, y];
    }

    static public Grid GetGrid(GridPos pos)
    {
        return GetGrid(pos.x, pos.y);
    }

    static public List<GridPos> GetGrids(float x, float y, float range)
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
                float len = instance.gridSize * 0.71f + range;
                if ((v - pos).magnitude < len)
                {
                    GridPos grid_pos = new GridPos(i, j);
                    objList.Add(grid_pos);
                }
            }
        }

        return objList;
    }
}
