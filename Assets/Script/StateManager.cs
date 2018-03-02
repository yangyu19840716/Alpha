using System.Collections.Generic;

public enum StateState
{
    ENTER,
    TICK,
    LEAVE,
    //USER,
    ALL,
    NONE = -1
}

public delegate void DeleAction();

public class State
{
    int _stateName = -1;
    DeleAction[] _actions = new DeleAction[(int)StateState.ALL];

    public State(int name, DeleAction tick = null, DeleAction enter = null, DeleAction leave = null)
    {
        _stateName = name;
        _actions[(int)StateState.ENTER] = enter;
        _actions[(int)StateState.TICK] = tick;
        _actions[(int)StateState.LEAVE] = leave;
    }

    public void SetFunc(StateState state, DeleAction action)
    {
        _actions[(int)state] = action;
    }

    public int GetStateName()
    {
        return _stateName;
    }

    public void Action(StateState state)
    {
        DeleAction action = _actions[(int)state];
        if (action != null)
            action();
    }
}

public class StateMachine
{
    Dictionary<int, State> _stateMap = new Dictionary<int, State>();
    string _machineName = null;
    State _crtState = null;
    
    public StateMachine(string name)
    {
        _machineName = name;
        StateMachineManager.GetInstance().AddMachine(name, this);
    }

    public bool ToState(int name)
    {
        if (!_stateMap.ContainsKey(name))
            return false;

        if (_crtState != null)
        {
            if (name == _crtState.GetStateName())
                return false;

            _crtState.Action(StateState.LEAVE);
        }

        _crtState = _stateMap[name];
        _crtState.Action(StateState.ENTER);
        return true;
    }
    
    public void Tick()
    {
        if (_crtState != null)
            _crtState.Action(StateState.TICK);
    }
    
    public void AddState(int name, DeleAction tick = null, DeleAction enter = null, DeleAction leave = null)
    {
        State state = null;
        if (_stateMap.ContainsKey(name))
        {
            state = _stateMap[name];
            if (enter != null)
                state.SetFunc(StateState.ENTER, enter);
            if (tick != null)
                state.SetFunc(StateState.TICK, tick);
            if (leave != null)
                state.SetFunc(StateState.LEAVE, leave);
        }
        else
            _stateMap[name] = state = new State(name, tick, enter, leave);
    }    

    public void RemoveState(int name)
    {
        _stateMap.Remove(name);
    }
}

public class StateMachineManager
{
    Dictionary<string, StateMachine> _stateMachineMap = new Dictionary<string, StateMachine>();

    public static StateMachineManager GetInstance() { return Singleton<StateMachineManager>.GetInstance(); }

    public void Tick()
    {
        foreach (StateMachine m in _stateMachineMap.Values)
        {
            m.Tick();
        }
    }

    public void AddMachine(string name, StateMachine machine)
    {
        _stateMachineMap[name] = machine;
    }

    public void RemoveMachine(string name)
    {
        _stateMachineMap.Remove(name);
    }
}
