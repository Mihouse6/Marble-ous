using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InputManager : MonoBehaviour
{
    public void PauseButton_OnClick()
    {
        GameManager.instance.isPaused = !GameManager.instance.isPaused;

        if (GameManager.instance.isPaused)
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = true;
            Time.timeScale = 0;
        }
        else
        {
            Camera.main.GetComponent<PostProcessVolume>().enabled = false;
            Time.timeScale = 1;
        }
    }
}
