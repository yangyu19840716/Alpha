﻿using UnityEngine;
using System.Collections.Generic;

class DebugModule
{
    public static Material lineMaterial = null;
    public static Material redlineMaterial = null;
    public static Material greenlineMaterial = null;
    public static GameObject circle = null;

    public static void GLDrawLine(Vector3 start, Vector3 end)
    {
        GL.PushMatrix();
        lineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
        GL.End();
        GL.PopMatrix();
    }

    public static void GLDrawRedLine(Vector3 start, Vector3 end)
    {
        GL.PushMatrix();
        redlineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
        GL.End();
        GL.PopMatrix();
    }

    public static void GLDrawGreenLine(Vector3 start, Vector3 end)
    {
        GL.PushMatrix();
        greenlineMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, end.z);
        GL.End();
        GL.PopMatrix();
    }

    public static void DrawEntityLine(Entity entity)
    {
        if (entity == null)
            return;

        for (int i = 0; i < entity.friendList.Count; i++)
        {
            Entity f = entity.friendList[i];
            DebugModule.GLDrawGreenLine(entity.obj.transform.position, f.obj.transform.position);
        }
        for (int i = 0; i < entity.enemyList.Count; i++)
        {
            Entity e = entity.enemyList[i];
            DebugModule.GLDrawRedLine(entity.obj.transform.position, e.obj.transform.position);
        }
    }

    public static void DrawWorldGrid()
    {
        Vector3 pos1 = new Vector3();
        Vector3 pos2 = new Vector3();
        Vector3 pos3 = new Vector3();
        Vector3 pos4 = new Vector3();
        pos1.y = pos2.y = pos3.y = pos4.y = 0.5f;
        float f = World.GetInstacne().worldSize * 0.5f;
        float gridSize = World.GetInstacne().gridSize;
        for (int i = 0; i <= GridPos.gridNum; i++)
        {
            pos1.x = pos3.z = -f + gridSize * i;
            pos1.z = pos3.x = -f;
            pos2.x = pos4.z = -f + gridSize * i;
            pos2.z = pos4.x = f;
            DebugModule.GLDrawLine(pos1, pos2);
            DebugModule.GLDrawLine(pos3, pos4);
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
        float gridSize = World.GetInstacne().gridSize;
        List<GridPos> list = World.GetInstacne().GetGrids(entity.obj.transform.position.x, entity.obj.transform.position.z, entity.GetData().range);
        for (int i = 0; i < list.Count; i++)
        {
            Vector2 pos = World.GetInstacne().GetGridCenter(list[i]);
            pos1.x = pos4.x = pos.x - gridSize * 0.5f;
            pos1.z = pos3.z = pos.y - gridSize * 0.5f;
            pos2.x = pos3.x = pos.x + gridSize * 0.5f;
            pos2.z = pos4.z = pos.y + gridSize * 0.5f;
            GLDrawLine(pos1, pos2);
            GLDrawLine(pos3, pos4);
        }
    }

    public static void ShowCircle(Vector3 pos, float r)
    {
        if (circle == null)
            return;

        Renderer circleRenderer = circle.GetComponent<Renderer>();
        circleRenderer.enabled = false;

        circle.transform.position = pos;
        circle.transform.localScale = new Vector3(r, 0.0f, r);
        circleRenderer.enabled = true;
    }

    public static void hideCircle()
    {
        if (circle == null)
            return;

        Renderer circleRenderer = circle.GetComponent<Renderer>();
        circleRenderer.enabled = false;
    }
}