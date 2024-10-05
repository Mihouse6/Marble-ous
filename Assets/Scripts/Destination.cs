using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Destination Collision");

        //If colliding object is the marble, finish the level
        if (other.gameObject.CompareTag("Marble"))
            GameManager.instance.FinishLevel();
    }
}
