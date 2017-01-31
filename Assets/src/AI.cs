using UnityEngine;
using System.Collections.Generic;

public enum AIState { IDLE, MOVE, ALL, NONE = -1 }

public class AI : MonoBehaviour
{
    Entity owner = null;
    StateMachine machine = null;

    public void Init(Entity o, string name)
    {
        owner = o;
        machine = new StateMachine(name);
        machine.AddState((int)AIState.IDLE, () => owner.StartIdle(), () => owner.Idling(), () => owner.EndIdle());
        machine.AddState((int)AIState.MOVE, () => owner.StartMove(), () => owner.Moving(), () => owner.EndMove());
        machine.ToState((int)AIState.IDLE);
    }

    public Entity GetOwner()
    {
        return owner;
    }

    public void ToState(AIState state)
    {
        machine.ToState((int)state);
    }
}
