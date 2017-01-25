using UnityEngine;

public class Entry : MonoBehaviour
{
    public GameObject redCube = null;
    public GameObject blueCube = null;
    public GameObject circle = null;
    public Material lineMat = null;
    public Material redlineMat = null;
    public Material greenlineMat = null;
    public Material selectMat = null;
    public float sceneSize = 100.0f;
    public float gridSize = 10.0f;

    public static Entry GetInstacne() { return Singleton<Entry>.GetInstacne(); }

    void Start ()
    {
        DebugModule.lineMaterial = lineMat;
        DebugModule.redlineMaterial = redlineMat;
        DebugModule.greenlineMaterial = greenlineMat;
        Entity.red = redCube;
        Entity.blue = blueCube;
        Entity.selectMat = selectMat;
        DebugModule.circle = circle;
        SceneManager.GetInstacne().Init(sceneSize, gridSize);
	}
	
	void Update ()
    {
        SceneManager.GetInstacne().Update();
    }

    void OnPostRender()
    {
        Entity entity = SceneManager.GetInstacne().pickedEntity;
        if (entity != null)
            DebugModule.ShowCircle(entity.obj.transform.position, entity.GetData().range * 2);
        DebugModule.DrawWorldGrid();
        DebugModule.DrawEntityGrid(entity);
        DebugModule.DrawEntityLine(entity);
    }
}
