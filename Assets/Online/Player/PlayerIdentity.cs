using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerIdentity : NetworkBehaviour
{
    public string Name { get { return playerName; } }
    public ushort Id { get { return playerId; } }

    [SyncVar]
    private string playerName;
    [SyncVar]
    private ushort playerId;

    public static List<PlayerIdentity> Players { get; private set; } = new List<PlayerIdentity>();

    private void Awake() 
    { 
        Players.Add(this);
        SetNameAndId();
    }
    private void OnDestroy() => Players.Remove(this);

    [ServerCallback]
    private void SetNameAndId()
    {
        ushort id = (ushort)Players.Count;
        playerName = "Player_" + id;
        playerId = id;
    }
}
