using UnityEngine;
using System.Collections.Generic;

class GridData
{
    public int[] risk = new int[(int)EntityType.ALL];
    public int resource = 100;
}

public class Grid
{
    public List<Entity>[] entityList = new List<Entity>[(int)EntityType.ALL];
    public bool dirty = false;
    public Vector2 centerPos;

    public Grid()
    {
        for (int i = 0; i < (int)EntityType.ALL; i++)
        {
            entityList[i] = new List<Entity>();
        }
    }
    
    public void AddEntity(Entity entity)
    {
        entityList[(int)entity.GetData().type].Add(entity);
        dirty = true;
    }

    public void RemoveEntity(Entity entity)
    {
        entityList[(int)entity.GetData().type].Remove(entity);
        dirty = true;
    }

    public int GetEntityNum(EntityType type = EntityType.ALL)
    {
        if (type != EntityType.ALL)
            return entityList[(int)type].Count;

        int count = 0;
        for(int i = 0;  i < (int)EntityType.ALL; i++)
        {
            count += entityList[i].Count;
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
            count += entityList[i].Count;
        }
        return count;
    }
};

public class GridPos
{
    static int gridNum = 0;
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

    public static void SetGridNum(int num)
    {
        gridNum = num;
    }

    public int x, y;
}

public class World
{
    public float worldSize = 0.0f;
    public float gridSize = 0.0f;
    public int gridNum = 0;
    public Grid[,] grid = null;

    public static World GetInstance() { return Singleton<World>.GetInstance(); }

    public void Update()
    {
    }

    public void AddToField(Entity entity)
    {
        entity.gridPos = PosToGridPos(entity.obj.transform.position.x, entity.obj.transform.position.z);
        grid[entity.gridPos.x, entity.gridPos.y].AddEntity(entity);
    }

    public void RemoveFromField(Entity entity)
    {
        grid[entity.gridPos.x, entity.gridPos.y].RemoveEntity(entity);
    }

    public void CreateWorld(float world_size, float grid_size)
    {
        worldSize = world_size;
        gridSize = grid_size;
        gridNum = (int)(worldSize / gridSize + 1.0f - Utility.MIN_FLOAT);
        GridPos.SetGridNum(gridNum);
        grid = new Grid[gridNum, gridNum];
        for (int i = 0; i < gridNum; i++)
        {
            for (int j = 0; j < gridNum; j++)
            {
                Grid g = new Grid();
                g.centerPos = GridPosToCenterPos(i, j);
                grid[i, j] = g;
            }
        }
    }

    public GridPos PosToGridPos(float x, float y)
    {
        return new GridPos((int)((x + worldSize * 0.5f) / gridSize), (int)((y + worldSize * 0.5f) / gridSize));
    }

    public GridPos PosToGridPos(Vector2 pos)
    {
        return PosToGridPos(pos.x, pos.y);
    }

    public Vector2 GridPosToCenterPos(int gx, int gy)
    {
        return new Vector2((gx + 0.5f) * gridSize - worldSize * 0.5f, (gy + 0.5f) * gridSize - worldSize * 0.5f);
    }

    public Vector2 GetGridCenter(int gx, int gy)
    {
        return grid[gx, gy].centerPos;
    }

    public Vector2 GetGridCenter(GridPos gridPos)
    {
        return GetGridCenter(gridPos.x, gridPos.y);
    }

    public Grid GetGrid(int x, int y)
    {
        return grid[x, y];
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
                float len = gridSize * 0.71f /* 2.0f ^ 0.5f * 0.5f */ + range;
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
