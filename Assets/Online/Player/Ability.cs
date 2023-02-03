using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : NetworkBehaviour
{
    public abstract void Use();
    public bool InProcess { get; protected set; }
}
