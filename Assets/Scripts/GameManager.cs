using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameOver = false;
    public bool isActive = true;
    private Animator animator;
    public CarController car;

    public void GameOver()
    {
        isGameOver = true;
        car.im.DisableInput();
    }

    // Update is called once per frame
    void Update()
    {
        // Restart the game if the game is over and when 'R' is pressed
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
