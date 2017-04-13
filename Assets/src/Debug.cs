using UnityEngine;
using System.Collections.Generic;

class DebugModule
{
//    public static Material lineMaterial = new Material(Shader.Find("Standard"));
    public static GameObject _circle = null;

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        Debug.DrawLine(start, end, color);
//         GL.PushMatrix();
//         lineMaterial.SetPass(0);
//         GL.Begin(GL.LINES);
//         GL.Vertex3(start.x, start.y, start.z);
//         GL.Vertex3(end.x, end.y, end.z);
//         GL.End();
//         GL.PopMatrix();
    }

    public static void DrawEntityLine(Entity entity)
    {
        if (entity == null)
            return;

        foreach (Entity f in entity._friendList)
        {
            DrawLine(entity._obj.transform.position, f._obj.transform.position, Color.green);
        }
        foreach (Entity e in entity._enemyList)
        {
            DrawLine(entity._obj.transform.position, e._obj.transform.position, Color.red);
        }
    }

    public static void DrawWorldGrid()
    {
        Vector3 pos1 = new Vector3();
        Vector3 pos2 = new Vector3();
        Vector3 pos3 = new Vector3();
        Vector3 pos4 = new Vector3();
        pos1.y = pos2.y = pos3.y = pos4.y = 0.5f;
        float f = World.GetInstance()._worldSize * 0.5f;
        float gridSize = World.GetInstance()._gridSize;
        for (int i = 0; i <= World.GetInstance()._gridNum; i++)
        {
            pos1.x = pos3.z = -f + gridSize * i;
            pos1.z = pos3.x = -f;
            pos2.x = pos4.z = -f + gridSize * i;
            pos2.z = pos4.x = f;
            DrawLine(pos1, pos2, Color.black);
            DrawLine(pos3, pos4, Color.black);
        }
    }

    public static void DrawEntityGrid(Entity entity)
    {
        if (entity == null)
            return;

        Vector3 pos1 = new Vector3();
        Vector3 pos2 = new Vector3();
        Vector3 pos3 = new Vector3();
        Vector3 pos4 = new Vector3();
        pos1.y = pos2.y = pos3.y = pos4.y = 0.5f;
        float gridSize = World.GetInstance()._gridSize;
        List<GridPos> list = World.GetInstance().GetGrids(entity._obj.transform.position.x, entity._obj.transform.position.z, entity.GetCrtData()._range);
        foreach (GridPos gridPos in list)
        {
            Vector2 pos = World.GetInstance().GetGridCenter(gridPos);
            pos1.x = pos4.x = pos.x - gridSize * 0.5f;
            pos1.z = pos3.z = pos.y - gridSize * 0.5f;
            pos2.x = pos3.x = pos.x + gridSize * 0.5f;
            pos2.z = pos4.z = pos.y + gridSize * 0.5f;
            DrawLine(pos1, pos2, Color.black);
            DrawLine(pos3, pos4, Color.black);
        }
    }

    public static void ShowCircle(Entity entity, float r)
    {
        if (_circle == null)
            return;

        Renderer circleRenderer = _circle.GetComponent<Renderer>();
        circleRenderer.enabled = true;

        _circle.transform.parent = entity._obj.transform;
        _circle.transform.localPosition = new Vector3(0.0f, -0.1f, 0.0f);
        _circle.transform.localScale = new Vector3(r, 0.0f, r);
    }

    public static void HideCircle()
    {
        if (_circle == null)
            return;

        Renderer circleRenderer = _circle.GetComponent<Renderer>();
        circleRenderer.enabled = false;
    }

    public static void DebugDraw()
    {
        Entity entity = SceneManager.GetInstance()._pickedEntity;
        if (entity != null)
        {
            DrawEntityLine(entity);
            DrawEntityGrid(entity);
        }

        DrawWorldGrid();
    }
}