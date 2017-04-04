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
    StateState state = StateState.NONE;
    int stateName;
    DeleAction[] actions = new DeleAction[(int)StateState.ALL];

    public State(int name, DeleAction enter = null, DeleAction tick = null, DeleAction leave = null)
    {
        stateName = name;
        actions[(int)StateState.ENTER] = enter;
        actions[(int)StateState.TICK] = tick;
        actions[(int)StateState.LEAVE] = leave;
    }

    public void SetFunc(StateState state, DeleAction action)
    {
        actions[(int)state] = action;
    }

    public int GetStateName()
    {
        return stateName;
    }

    //     public void Enter()
    //     {
    //         Action(StateState.ENTER);
    //     }
    // 
    //     public void Tick()
    //     {
    //         Action(StateState.TICK);
    //     }
    // 
    //     public void Leave()
    //     {
    //         Action(StateState.LEAVE);
    //     }

    public void Action(StateState state)
    {
        DeleAction action = actions[(int)state];
        if (action != null)
            action();
    }
}

public class StateMachine
{
    Dictionary<int, State> stateMap = new Dictionary<int, State>();
    string machineName = null;
    State crtState = null;
    
    public StateMachine(string name)
    {
        machineName = name;
        StateMachineManager.GetInstance().AddMachine(name, this);
    }

    public bool ToState(int name)
    {
        if (!stateMap.ContainsKey(name))
            return false;

        if (crtState != null && name == crtState.GetStateName())
            return false;

        if (crtState != null)
        {
            crtState.Action(StateState.LEAVE);
        }

        crtState = stateMap[name];
        crtState.Action(StateState.ENTER);
        return true;
    }
    
    public void Tick()
    {
        if (crtState != null)
            crtState.Action(StateState.TICK);
    }
    
    public void AddState(int name, DeleAction tick = null, DeleAction enter = null, DeleAction leave = null)
    {
        State state = null;
        if (stateMap.ContainsKey(name))
        {
            state = stateMap[name];
            if (enter != null)
                state.SetFunc(StateState.ENTER, enter);
            if (tick != null)
                state.SetFunc(StateState.TICK, tick);
            if (leave != null)
                state.SetFunc(StateState.LEAVE, leave);
        }
        else
            stateMap[name] = state = new State(name, enter, tick, leave);
    }    

    public void RemoveState(int name)
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
