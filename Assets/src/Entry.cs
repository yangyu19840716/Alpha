using UnityEngine;

public class Entry : MonoBehaviour
{
    public GameObject _cube = null;
    public GameObject _circle = null;
    public float _sceneSize = 100.0f;
    public float _gridSize = 10.0f;

    public static Entry GetInstance() { return Singleton<Entry>.GetInstance(); }

    void Start ()
    {
        DebugModule._circle = _circle;
        Entity._cube = _cube;
        SceneManager.GetInstance().Init(_sceneSize, _gridSize);
	}
	
	void Update ()
    {
        SceneManager.GetInstance().Update();
        DebugModule.DebugDraw();
    }
}
