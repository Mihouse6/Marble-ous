using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;

public class HapticsManager : MonoBehaviour
{
    //Singleton Setup
    public static HapticsManager instance = null;

    [HideInInspector] public bool VibrationEnabled = true;

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

    private void Start()
    {
        VibrationEnabled = true;
    }

    public void DefaultVibration()
    {
        if (VibrationEnabled)
            Handheld.Vibrate();
    }

    public void LightVibration()
    {
        if (VibrationEnabled)
            HapticFeedback.LightFeedback();
    }

    public void MediumVibration()
    {
        if (VibrationEnabled)
            HapticFeedback.MediumFeedback();
    }

    public void HeavyVibration()
    {
        if (VibrationEnabled)
            HapticFeedback.HeavyFeedback();
    }
}
