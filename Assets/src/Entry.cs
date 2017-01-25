using UnityEngine;

public class Entry : MonoBehaviour
{
    public GameObject cube = null;
    public GameObject circle = null;
    public float sceneSize = 100.0f;
    public float gridSize = 10.0f;

    public static Entry GetInstance() { return Singleton<Entry>.GetInstance(); }

    void Start ()
    {
        DebugModule.circle = circle;
        Entity.cube = cube;
        SceneManager.GetInstance().Init(sceneSize, gridSize);
	}
	
	void Update ()
    {
        SceneManager.GetInstance().Update();

        Entity entity = SceneManager.GetInstance().pickedEntity;
        if (entity != null)
        {
            DebugModule.ShowCircle(entity.obj.transform.position, entity.GetData().range * 2);
            DebugModule.DrawEntityLine(entity);
            DebugModule.DrawEntityGrid(entity);
        }
        else
        {
            DebugModule.hideCircle();
        }

        DebugModule.DrawWorldGrid();
    }
}
