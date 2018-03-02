using UnityEngine;

public class Entry : MonoBehaviour
{
    public float _sceneSize = GameConst.SCENE_SIZE;
    public float _gridSize = GameConst.GRID_SIZE;

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
