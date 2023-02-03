using UnityEngine;

public class HelpGUI : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20f, 160f, 450f, 500f));
        GUILayout.Box($"Press \"Host\" to create a room\n" +
            $"Press \"Client\" to connect to existing room\n\n" +
            $"Hold right mouse button to rotate camera\n" +
            $"Press left mouse button to use \"Impulse\" ability\n" +
            $"Press WASD to move around\n" +
            $"Press Q and E to rotate your character\n" +
            $"Press Space to jump\n\n" +
            $"Note that Impulse ability only works in the direction of motion\n" +
            $"No motion - no Impulse");
        GUILayout.EndArea();
    }
}
