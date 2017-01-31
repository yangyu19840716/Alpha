using UnityEngine;
using System.Collections.Generic;

public class SceneManager
{
    public Entity pickedEntity = null;

    bool bInit = false;

    List<Entity> entityList = new List<Entity>();

    public static SceneManager GetInstance() { return Singleton<SceneManager>.GetInstance(); }

    public void Init (float scene_size, float grid_size) {
        if (bInit)
            return;

        Entity.StaticInit();

        World.GetInstance().CreateWorld(scene_size, grid_size);
    
        for (int i = 0; i < GameConst.ENTITY_NUM; i++)
        {
            float x = Random.value * 0.5f * GameConst.ENTITY_NUM - GameConst.ENTITY_NUM * 0.1f;
            float y = (Random.value - 0.5f) * GameConst.ENTITY_NUM * 0.9f;
            Entity entity = new Entity(EntityType.RED, x, y, "Red_" + i);
            World.GetInstance().AddToWorld(entity);
            entityList.Add(entity);

            x = -Random.value * 0.5f * GameConst.ENTITY_NUM + GameConst.ENTITY_NUM * 0.1f;
            y = (Random.value - 0.5f) * GameConst.ENTITY_NUM * 0.9f;
            entity = new Entity(EntityType.BLUE, x, y, "Blue_" + i);
            World.GetInstance().AddToWorld(entity);
            entityList.Add(entity);
        }

        bInit = true;
	}

    public void Update()
    {
        StateMachineManager.GetInstance().Tick();

        foreach (Entity entity in entityList)
        {
            entity.Update();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit))
            {
                if (pickedEntity != null)
                    pickedEntity.Unpicked();

                pickedEntity = rayhit.collider.gameObject.GetComponent<AI>().GetOwner();
                pickedEntity.Picked();
            }
            else
            {
                if (pickedEntity != null)
                    pickedEntity.Unpicked();
                pickedEntity = null;
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
}

