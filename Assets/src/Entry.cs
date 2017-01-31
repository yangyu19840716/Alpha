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
        DebugModule.DebugDraw();
    }
}
