using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ScoreGUI : MonoBehaviour
{
    private void OnGUI()
    {
        if(ScoreSystem.Scores == null)
        {
            return;
        }
        GUILayout.BeginArea(new Rect(20f, 100f, 180f, 30f));
        GUILayout.Box($"Score:");
        GUILayout.EndArea();
        short offset = 1;
        foreach (var score in ScoreSystem.Scores)
        {
            GUILayout.BeginArea(new Rect(20f, 100f + (offset * 35), 180f, 30f));
            GUILayout.Box($"{score.PlayerIdentityInGame.Name}: {score.MyScore}");
            GUILayout.EndArea();
            offset++;
        }
    }
}
