using UnityEngine;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    Entity owner = null;

    public void Init(Entity o)
    {
        owner = o;
    }

    public Entity GetOwner()
    {
        return owner;
    }
}
