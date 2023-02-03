using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    private void Awake()
    {
        ConfigureScreen();
    }

    private static void ConfigureScreen()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Screen.SetResolution(Screen.width, Screen.height, true);
    }
}
