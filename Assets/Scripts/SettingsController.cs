using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle VibrationToggle;

    private void Start()
    {
        MusicSlider.value = 1;
        SFXSlider.value = 1;
        VibrationToggle.isOn = true;
    }

    public void TogglePauseMenu()
    {
        PauseMenu.SetActive(!PauseMenu.activeSelf);
    }

    public void MusicSlider_OnValueChanged()
    {
        AudioManager.instance.SetMusicVolume(MusicSlider.value);
    }

    public void SFXSlider_OnValueChanged()
    {
        AudioManager.instance.SetSFXVolume(SFXSlider.value);
    }

    public void VibrationToggle_OnValueChanged()
    {
        HapticsManager.instance.VibrationEnabled = VibrationToggle.isOn;
        HapticsManager.instance.DefaultVibration();
    }
}
