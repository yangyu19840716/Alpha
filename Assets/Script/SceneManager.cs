using UnityEngine;
using System.Collections.Generic;

public class SceneManager
{
    bool _bInit = false;
    List<Entity> _entityList = new List<Entity>();

    public Entity _pickedEntity = null;
    public float _crtTime = 0.0f;

    public static SceneManager GetInstance() { return Singleton<SceneManager>.GetInstance(); }


    public void Init (float sceneSize, float gridSize) {
        if (_bInit)
            return;

        World.GetInstance().CreateWorld(sceneSize, gridSize);
    
        for (int i = 0; i < GameConst.ENTITY_NUM; i++)
        {
            float x = RandomModule.Rand(0.0f, 0.5f) * GameConst.ENTITY_NUM - GameConst.ENTITY_NUM * 0.1f;
            float y = (RandomModule.Rand(-0.5f, 0.5f)) * GameConst.ENTITY_NUM * 0.9f;
            Entity entity = new Entity(EntityType.RED, x, y, "Red_" + i);
            World.GetInstance().Add(entity);
            _entityList.Add(entity);

            x = -RandomModule.Rand(0.0f, 0.5f) * GameConst.ENTITY_NUM + GameConst.ENTITY_NUM * 0.1f;
            y = (RandomModule.Rand(-0.5f, 0.5f)) * GameConst.ENTITY_NUM * 0.9f;
            entity = new Entity(EntityType.BLUE, x, y, "Blue_" + i);
            World.GetInstance().Add(entity);
            _entityList.Add(entity);
        }

        _bInit = true;
	}

    public void Update()
    {
        _crtTime += Time.deltaTime;

        StateMachineManager.GetInstance().Tick();

        foreach (Entity entity in _entityList)
        {
            entity.UpdateGrid();
        }

        foreach (Entity entity in _entityList)
        {
            entity.Update();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit))
            {
                if (_pickedEntity != null)
                    _pickedEntity.Unpicked();

                _pickedEntity = rayhit.collider.gameObject.GetComponent<AI>().GetOwner();
                _pickedEntity.Picked();
            }
            else
            {
                if (_pickedEntity != null)
                    _pickedEntity.Unpicked();
                _pickedEntity = null;
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

