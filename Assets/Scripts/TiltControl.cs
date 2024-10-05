using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TiltControl : MonoBehaviour
{
    //DEBUG
    public TextMeshProUGUI XRotText;
    public TextMeshProUGUI YRotText;
    public TextMeshProUGUI ZRotText;

    private void Start()
    {
        // Make sure device supprts Gyroscope
        if (!SystemInfo.supportsGyroscope) throw new Exception("Device does not support Gyroscope");

        Input.gyro.enabled = true;
    }

    private void Update()
    {
        Vector3 rotEuler = Input.gyro.attitude.eulerAngles;

        transform.rotation = Quaternion.Euler(-rotEuler.x, -rotEuler.z, -rotEuler.y);

        XRotText.text = "X Rot: " + transform.rotation.eulerAngles.x;
        YRotText.text = "Y Rot: " + transform.rotation.eulerAngles.y;
        ZRotText.text = "Z Rot: " + transform.rotation.eulerAngles.z;
    }
}
