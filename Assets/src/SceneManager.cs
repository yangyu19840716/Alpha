using UnityEngine;
using System.Collections.Generic;

public class SceneManager {
    static SceneManager instance = null;

    public Entity pickedEntity = null;
    public GameObject circle = null;

    bool bInit = false;
    Field field = null;
    Renderer circleRenderer = null;

    float fieldSize = 0.0f;
    float gridSize = 0.0f;
    List<Entity> entityList = new List<Entity>();
    List<Entity> dyingList = new List<Entity>();
    bool bDrawGrid = false;

    SceneManager()
    {
        if(bInit)
            instance = this;
    }

    public static SceneManager GetInstance()
    {
        if (instance == null)
            instance = new SceneManager();
        return instance;
    }

    public void Init (float scene_size, float grid_size) {
        if (bInit)
            return;

        fieldSize = scene_size;
        gridSize = grid_size;
        field = Field.CreateField(scene_size, grid_size);
        circleRenderer = circle.GetComponent<Renderer>();
        circleRenderer.enabled = false;

        for (int i = 0; i < GameConst.ENTITY_NUM; i++)
        {
            float x = Random.value / 2 * GameConst.ENTITY_NUM - GameConst.ENTITY_NUM * 0.1f;
            float y = (Random.value - 0.5f) * GameConst.ENTITY_NUM * 0.9f;
            Entity entity = new Entity(Entity.Type.RED, x, y, "Red_" + i);
            Field.AddToField(entity);
            entityList.Add(entity);

            x = -Random.value / 2 * GameConst.ENTITY_NUM + GameConst.ENTITY_NUM * 0.1f;
            y = (Random.value - 0.5f) * GameConst.ENTITY_NUM * 0.9f;
            entity = new Entity(Entity.Type.BLUE, x, y, "Blue_" + i);
            Field.AddToField(entity);
            entityList.Add(entity);
        }

        for (int i = 0; i < entityList.Count; i++)
        {
            entityList[i].Init();
        }

        bInit = true;
	}

    public void Update()
    {
        field.Update();
        for (int i = 0; i < entityList.Count; i++)
        {
            entityList[i].Update();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit))
            {
                if (pickedEntity != null)
                    pickedEntity.Unpicked();

                pickedEntity = rayhit.collider.gameObject.GetComponent<AI>().owner;
                pickedEntity.Picked();
                float range = pickedEntity.GetRange() * 2;
                circle.transform.localScale = new Vector3(range, 0.0f, range);
                circle.transform.position = pickedEntity.obj.transform.position;
                circleRenderer.enabled = true;
            }
            else
            {
                if (pickedEntity != null)
                    pickedEntity.Unpicked();
                pickedEntity = null;
                circleRenderer.enabled = false;
            }
        }

        Vector3 pos = Camera.main.transform.position;
        if (Input.GetKey(KeyCode.RightArrow))
            pos.x += 1.0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            pos.x -= 1.0f;
        if (Input.GetKey(KeyCode.UpArrow))
            pos.z += 1.0f;
        if (Input.GetKey(KeyCode.DownArrow))
            pos.z -= 1.0f;
        if (Input.GetKey(KeyCode.PageUp))
            pos.y += 1.0f;
        if (Input.GetKey(KeyCode.PageDown))
            pos.y -= 1.0f;
        if (Input.GetKey(KeyCode.Space))
        {
            pos.x = 0.0f;
            pos.y = 30.0f;
            pos.z = 0.0f;
        }
        Camera.main.transform.position = pos;
    }

    public void DrawLine()
    {
        if(pickedEntity != null)
        {
            for (int i = 0; i < pickedEntity.friendList.Count; i++)
            {
                Entity f = pickedEntity.friendList[i];
                Utility.GLDrawGreenLine(pickedEntity.obj.transform.position, f.obj.transform.position);
            }
            for (int i = 0; i < pickedEntity.enemyList.Count; i++)
            {
                Entity e = pickedEntity.enemyList[i];
                Utility.GLDrawRedLine(pickedEntity.obj.transform.position, e.obj.transform.position);
            }
        }
    }

    public void DrawGrid()
    {
        if (!bDrawGrid)
            return;

        Vector3 pos1 = new Vector3();
        Vector3 pos2 = new Vector3();
        Vector3 pos3 = new Vector3();
        Vector3 pos4 = new Vector3();
        pos1.y = pos2.y = pos3.y = pos4.y = 0.5f;
        float f = fieldSize / 2;
        for (int i = 0; i <= GridPos.gridNum; i++)
        {
            pos1.x = pos3.z = - f + gridSize * i;
            pos1.z = pos3.x = - f;
            pos2.x = pos4.z = - f + gridSize * i;
            pos2.z = pos4.x = f;
            Utility.GLDrawLine(pos1, pos2);
            Utility.GLDrawLine(pos3, pos4);
        }

        if(pickedEntity != null)
        {
            List<GridPos> list = Field.GetGrids(pickedEntity.obj.transform.position.x, pickedEntity.obj.transform.position.z, pickedEntity.GetRange());
            for (int i = 0; i < list.Count; i++)
            {
                Vector2 pos = Field.GetGridCenter(list[i]);
                pos1.x = pos4.x = pos.x - gridSize / 2;
                pos1.z = pos3.z = pos.y - gridSize / 2;
                pos2.x = pos3.x = pos.x + gridSize / 2;
                pos2.z = pos4.z = pos.y + gridSize / 2;
                Utility.GLDrawLine(pos1, pos2);
                Utility.GLDrawLine(pos3, pos4);
            }
        }
    }

    public void EntityDie(Entity entity)
    {
        dyingList.Add(entity);
    }
}

