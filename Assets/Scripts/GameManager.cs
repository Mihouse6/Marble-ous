using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton Setup
    public static GameManager instance = null;

    public Grid Grid;

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

    //Finishes the current level and loads the next one
    public void FinishLevel()
    {
        Grid.NextLevel();
    }
}
