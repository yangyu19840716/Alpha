using UnityEngine;

public class Entry : MonoBehaviour
{
    public float _sceneSize = 100.0f;
    public float _gridSize = 10.0f;

    public static Entry GetInstance() { return Singleton<Entry>.GetInstance(); }

    void Start ()
    {
        DebugModule.StaticInit();
        SceneManager.GetInstance().Init(_sceneSize, _gridSize);
    }
	
	void Update ()
    {
        SceneManager.GetInstance().Update();
        DebugModule.DebugDraw();
    }
}
