using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Text startText;

    public void LoadGame()
    {
        // Load our game from start button
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        // Exit game from exit button
        Application.Quit();
    }
}
