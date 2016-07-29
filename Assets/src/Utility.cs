using UnityEngine;
using System.Collections.Generic;

public class Utility {
    public const float MIN_FLOAT = 0.000001f;
    public static Material lineMaterial = null;
    public static Material redlineMaterial = null;
    public static Material greenlineMaterial = null;
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
}
