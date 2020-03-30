using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameManager Game;
    public TextMeshProUGUI speedText;
    public Texture2D speedometerNeedleTexture;
    private Rect speedometerNeedleRect;
    private Vector2 needlePivot;

    public GameObject GameOverPanel;
    public GameObject outOfBoundsText;

    private float needleAngle = 0; // Should always be 0
    public float zeroAngle = -130; // zeroAngle is the angle of the needle, when the vehicle is not moving
    public float speedAddValue = 1.44f; // This is how much the needle will move in degrees for each kilometer.

    private float _speed;

    private void Start()
    {
        Game = GetComponent<GameManager>();
    }

    void OnGUI()
    {
        // Calculate the desired rects and the GUI size
        float speedometerMainSize = 0.512f * ((Screen.height / 3.5f) + (Screen.width / 3.5f));
        float speedometerNeedleSizeX = 10f;
        float speedometerNeedleSizeY = 70f;

        speedometerNeedleRect = new Rect(Screen.width - (speedometerMainSize / 2) - (speedometerNeedleSizeX / 2) + 30,
                                         Screen.height - (speedometerMainSize / 2) - (speedometerNeedleSizeY) + 30,
                                         speedometerNeedleSizeX, speedometerNeedleSizeY);

        // Pivot rotation Vector2
        needlePivot = new Vector2(speedometerNeedleRect.x + (speedometerNeedleSizeX / 2), speedometerNeedleRect.y + (speedometerNeedleSizeY));

        // zero_angle is the angle of the needle, when the vehicle is not moving
        if (!(_speed > 180))
        {
            needleAngle = zeroAngle + _speed * speedAddValue;
        }
        if (_speed < 0)
        {
            needleAngle = zeroAngle;
        }

        // Backup the GUI Matrix
        Matrix4x4 matrixBackup = GUI.matrix;

        // Do the actual rotation of the needle
        GUIUtility.RotateAroundPivot(needleAngle, needlePivot);

        // Draw the needle
        GUI.DrawTexture(speedometerNeedleRect, speedometerNeedleTexture);
        GUI.matrix = matrixBackup;
    }

    // Change speed for needle and speed text
    public virtual void changeSpeed(float speed)
    {
        _speed = speed;
        speedText.text = Mathf.Clamp(Mathf.Round(speed), 0f, 180f) + " KPH";
    }

    public void GameOver()
    {
        Game.GameOver();
        StartCoroutine(gameOverScreenDelay());
    }

    // Delay game over screen
    public IEnumerator gameOverScreenDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GameOverPanel.transform.localScale = new Vector3(1f, 1f);
    }

    // Display out of bounds
    public IEnumerator FlashOutOfBounds()
    {
        for (int i = 0; i < 3; i++)
        {
            outOfBoundsText.transform.localScale = new Vector3(1f, 1f);
            yield return new WaitForSeconds(0.5f);
            outOfBoundsText.transform.localScale = new Vector3(0f, 1f);
            yield return new WaitForSeconds(0.5f);
        }

        outOfBoundsText.transform.localScale = new Vector3(0f, 1f);
    }
}
