using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton Setup
    public static GameManager instance = null;

    //Awake Checks - Singleton setup
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(instance);
    }

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyDown("space"))
            FinishLevel();
        //DEBUG
    }

    //Finished the current level and reloads the scene
    public void FinishLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
