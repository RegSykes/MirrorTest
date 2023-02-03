using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreSystem : NetworkBehaviour
{
    [SerializeField] private ushort MaxScore = 3;
    [SerializeField] private float RestartTime = 5;

    [SyncVar]
    private ushort SyncMaxScore = 3;

    [SyncVar]
    private float SyncRestartTime = 5;

    public ushort MyScore { get; private set; } = 0;
    [SerializeField] private bool Winner = false;
    private float restartCounter = 0.255f;

    public static bool GameOver { get; private set; } = false;

    public static List<ScoreSystem> Scores { get; private set; } = new List<ScoreSystem>();
    [SerializeField] public PlayerIdentity PlayerIdentityInGame { get; private set; }

    private void Awake()
    {
        if (!PlayerIdentityInGame)
        {
            PlayerIdentityInGame = GetComponent<PlayerIdentity>();
        }
    }

    private void Start() => Scores.Add(this);

    private void OnDestroy() => Scores.Remove(this);

    [ClientRpc]
    public void IncrementScoreRpc()
    {
        if (GameOver)
        {
            return;
        }
        MyScore++;
        if (MyScore >= SyncMaxScore)
        {
            restartCounter = SyncRestartTime;
            Winner = true;
            GameOver = true;
        }
    }

    private void FixedUpdate()
    {
        if (Winner)
        {
            restartCounter -= Time.fixedDeltaTime;
            if (restartCounter <= 0)
            {
                Winner = false;
                GameOver = false;
                RestartGame();
            }
        }
        OnServerSyncMaxScore();
        OnServerSyncRestartTime();
    }

    [ServerCallback]
    private void OnServerSyncMaxScore()
    {
        if(MaxScore != SyncMaxScore)
        {
            SyncMaxScore = MaxScore;
        }
    }

    [ServerCallback]
    private void OnServerSyncRestartTime()
    {
        if(RestartTime != SyncRestartTime)
        {
            SyncRestartTime = RestartTime;
        }
    }

    [ServerCallback]
    private void RestartGame() => GameObject.FindObjectOfType<NetworkRoomManagerExt>().OnServerRestartGame();

    private void OnGUI()
    {
        if (Winner)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 3, Screen.height / 3, 180f, 60f));
            GUILayout.Box($"{PlayerIdentityInGame.Name} won! \n \nRestart in {restartCounter:N2}");
            GUILayout.EndArea();
        }
    }
}
