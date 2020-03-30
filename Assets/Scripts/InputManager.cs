using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float throttle;
    public float steer;
    public bool lightsEnabled = false;

    private bool inputEnabled = true;

    // Update is called once per frame
    void Update()
    {
        // Get user input if input gathering is enabled
        if (inputEnabled)
        {
            // Only get forward input
            if (Input.GetAxis("Vertical") > 0)
            {
                throttle = Input.GetAxis("Vertical");
            }
            steer = Input.GetAxis("Horizontal");
            lightsEnabled = Input.GetKeyDown(KeyCode.L);
        }
        else
        {
            throttle = 0;
            steer = 0;
            lightsEnabled = false;
        }
    }

    public void DisableInput()
    {
        inputEnabled = false;
    }
}
