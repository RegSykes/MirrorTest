using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRoomManagerExt : Mirror.NetworkRoomManager
{
    public override Transform GetStartPosition()
    {
        startPositions.RemoveAll(t => t == null);

        if (startPositions.Count == 0)
            return null;

        if (playerSpawnMethod == PlayerSpawnMethod.Random)
        {
            Transform startPosition = startPositions[UnityEngine.Random.Range(0, startPositions.Count)];
            startPositions.Remove(startPosition);
            return startPosition;
        }
        else
        {
            Transform startPosition = startPositions[startPositionIndex];
            startPositionIndex = (startPositionIndex + 1) % startPositions.Count;
            return startPosition;
        }
    }

    [ServerCallback]
    public void OnServerRestartGame()
    {
        ServerChangeScene(RoomScene);
        ServerChangeScene(GameplayScene);
    }

    public override void OnGUI()
    {
        if (!showRoomGUI)
            return;

        if (NetworkServer.active && Utils.IsSceneActive(GameplayScene))
        {
            GUILayout.BeginArea(new Rect(Screen.width - 150f, 10f, 140f, 80f));
            if (GUILayout.Button("Return to Room"))
                ServerChangeScene(RoomScene);
            else if (GUILayout.Button("Restart"))
            {
                OnServerRestartGame();
            }
            GUILayout.EndArea();
        }

        if (Utils.IsSceneActive(RoomScene))
            GUI.Box(new Rect(10f, 180f, 520f, 150f), "PLAYERS");
    }
}
