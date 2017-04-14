using UnityEngine;

public enum ActionState { IDLE, MOVE, ATTACK, ALL, ATTACKED, THINK, NONE = -1 }

public class AI : MonoBehaviour
{
    Entity _owner = null;
    StateMachine _machine = null;
    ActionState _lastAction = ActionState.NONE;
    float _tickTime = 0.0f;
    const float _idleTime = 3.0f;
    Vector3 _targetPos;
    Entity _target = null;

    public void Init(Entity owner, string name)
    {
        _owner = owner;
        _machine = new StateMachine(name);
        _machine.AddState((int)ActionState.THINK, () => Thinking());
        _machine.AddState((int)ActionState.IDLE, () => Idling(), () => StartState());
        _machine.AddState((int)ActionState.MOVE, () => Moving(), () => StartMove());
        //_machine.AddState((int)ActionState.ATTACK, () => Attacking(), () => StartAttack());
        //_machine.AddState((int)ActionState.ATTACKED, () => Attacked(), () => StartState());

        ToState(ActionState.THINK);
    }

    public Entity GetOwner()
    {
        return _owner;
    }

    public void ToState(ActionState state)
    {
        _lastAction = state;
        _machine.ToState((int)state);
    }

    public void Thinking()
    {
        const int allAction = (int)ActionState.ALL;
        float[] p = new float[allAction];
        p[0] = 0.0f;
        float dp = _lastAction != ActionState.NONE ? 0.5f / allAction : 0.0f; // delta percentage
        float r = RandomModule.Rand();
        int i = 1;
        ActionState s;
        for (; i < allAction; i++)
        {
            p[i] = p[i - 1] + i * 1.0f / allAction;
            if ((int)_lastAction != (i - 1))
                p[i] += dp / (allAction - 1);
            else
                p[i] -= dp;
            if (r < p[i])
            {
                s = (ActionState)i - 1;
                ToState(s);
                return;
            }
        }

        s = (ActionState)ActionState.ALL - 1;
        ToState(ActionState.ALL - 1);
    }

    public void FindTargetPos()
    {
        float gridSize = World.GetInstance()._gridSize;
        float worldSize = World.GetInstance()._worldSize * 0.5f;
        _targetPos = gameObject.transform.position + new Vector3((RandomModule.Rand() - 0.5f) * gridSize, 0.0f, (RandomModule.Rand() - 0.5f) * gridSize);
        if (_targetPos.x > worldSize)
        {
            _targetPos.x -= gridSize;
        }
        else if (_targetPos.x < -worldSize)
        {
            _targetPos.x += gridSize;
        }
        if (_targetPos.z > worldSize)
        {
            _targetPos.z -= gridSize;
        }
        else if (_targetPos.z < -worldSize)
        {
            _targetPos.z += gridSize;
        }
    }

    public void FindAttackTarget()
    {
        foreach (Entity e in _owner._enemyList)
        {
            Vector3 d = gameObject.transform.position - e._obj.transform.position;
            if (d.sqrMagnitude <= _owner.GetCrtData()._range * _owner.GetCrtData()._range)
            {
                _target = e;
                break;
            }
        }
    }

    public void Idling()
    {
        float dTime = SceneManager.GetInstance()._crtTime - _tickTime;
        if (dTime > _idleTime)
        {
            ToState(ActionState.THINK);
        }
    }

    public void StartMove()
    {
        StartState();
        FindTargetPos();
        _owner.GetCrtData()._speed = _owner.GetData()._speed * RandomModule.Rand(0.1f, 1.0f);
    }

    public void Moving()
    {
        float dTime = SceneManager.GetInstance()._crtTime - _tickTime;
        float step = _owner.GetCrtData()._speed * dTime;
        Vector3 d = _targetPos - gameObject.transform.position;
        if (d.sqrMagnitude > step * step)
        {
            gameObject.transform.position += d.normalized * step;
        }
        else
        {
            gameObject.transform.position = _targetPos;
            ToState(ActionState.THINK);
        }
    }

    public void StartAttack()
    {
        StartState();
        FindAttackTarget();
    }

    public void Attacking()
    {

    }

    public void Attacked()
    { }

    public void StartState()
    {
        _tickTime = SceneManager.GetInstance()._crtTime;
    }
}
