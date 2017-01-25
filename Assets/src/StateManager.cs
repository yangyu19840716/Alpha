using System.Collections.Generic;

public class State
{
    public enum _State
    {
        ENTER,
        TICK,
        LEAVE,
        //USER,
        ALL,
        NONE = -1
    }

    _State state = _State.NONE;
    string stateName;
    public delegate void DeleAction();
    DeleAction[] actions = new DeleAction[(int)_State.ALL];

    public State(string name, DeleAction enter = null, DeleAction tick = null, DeleAction leave = null)
    {
        stateName = name;
        actions[(int)_State.ENTER] = enter;
        actions[(int)_State.TICK] = tick;
        actions[(int)_State.LEAVE] = leave;
    }

    public void SetFunc(_State state, DeleAction action)
    {
        actions[(int)state] = action;
    }

    public string GetStateName()
    {
        return stateName;
    }

//     public void Enter()
//     {
//         Action(_State.ENTER);
//     }
// 
//     public void Tick()
//     {
//         Action(_State.TICK);
//     }
// 
//     public void Leave()
//     {
//         Action(_State.LEAVE);
//     }

    public void Action(_State state)
    {
        DeleAction action = actions[(int)state];
        if (action != null)
            action();
    }
}

public class StateMachine
{
    Dictionary<string, State> stateMap = new Dictionary<string, State>();
    State crtState = null;
    
    public bool ToState(string name)
    {
        if (!stateMap.ContainsKey(name))
            return false;

        if (crtState != null && name == crtState.GetStateName())
            return false;

        if (crtState != null)
        {
            crtState.Action(State._State.LEAVE);
        }

        crtState = stateMap[name];
        crtState.Action(State._State.ENTER);
        return true;
    }
    
    public void Tick()
    {
        if (crtState != null)
            crtState.Action(State._State.TICK);
    }
    
    public void AddState(string name, State state)
    {
        stateMap[name] = state;
    }    

    public void RemoveState(string name)
    {
        stateMap.Remove(name);
    }
}

public class StateMachineManager
{
    Dictionary<string, StateMachine> stateMachineMap = new Dictionary<string, StateMachine>();

    public static StateMachineManager GetInstance() { return Singleton<StateMachineManager>.GetInstance(); }

    public void Tick()
    {
        foreach (StateMachine m in stateMachineMap.Values)
        {
            m.Tick();
        }
    }

    public void AddMachine(string name, StateMachine machine)
    {
        stateMachineMap[name] = machine;
    }

    public void RemoveMachine(string name)
    {
        stateMachineMap.Remove(name);
    }
}
