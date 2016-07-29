using UnityEngine;
using System.Collections.Generic;

public class Entry : MonoBehaviour {
    public GameObject redCube = null;
    public GameObject blueCube = null;
    public GameObject circle = null;
    public Material lineMat = null;
    public Material redlineMat = null;
    public Material greenlineMat = null;
    public Material selectMat = null;
    public float sceneSize = 100.0f;
    public float gridSize = 10.0f;

    static public Entry instance = null;

    SceneManager sceneMgr = null;
    // Use this for initialization
    void Start () {
        instance = this;
        Utility.lineMaterial = lineMat;
        Utility.redlineMaterial = redlineMat;
        Utility.greenlineMaterial = greenlineMat;
        Entity.red = redCube;
        Entity.blue = blueCube;
        Entity.selectMat = selectMat;
        sceneMgr = SceneManager.GetInstance();
        sceneMgr.circle = circle;
        sceneMgr.Init(sceneSize, gridSize);
	}
	
	// Update is called once per frame
	void Update () {
        sceneMgr.Update();
    }

    void OnPostRender()
    {
        sceneMgr.DrawLine();
        sceneMgr.DrawGrid();
}
}
