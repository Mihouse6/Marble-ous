using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Retireved 16/09/24 from https://stackoverflow.com/questions/42141056/how-to-detect-android-phone-rotation-in-unity

public class Orientation : MonoBehaviour
{
    //DEBUG
    public TextMeshProUGUI XRotText;
    public TextMeshProUGUI YRotText;
    public TextMeshProUGUI ZRotText;

    private Gyroscope gyro;
    private float initY;

    void Start()
    {
        // Make sure device supprts Gyroscope
        if (!SystemInfo.supportsGyroscope) throw new Exception("Device does not support Gyroscopoe");

        gyro = Input.gyro;
        gyro.enabled = true;    // Must enable the gyroscope

        Quaternion attitude = Attitude();
        initY = Rotation(attitude);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTilt();
    }

    private Quaternion Attitude()
    {
        Quaternion rawAttitude = gyro.attitude;             // Get the phones attitude in phone space
        Quaternion attitude = GyroToUnity(rawAttitude);     // Convert phone space to Unity space

        return attitude;
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return Quaternion.Euler(90, 0, 0) * new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private static float Rotation(Quaternion attitude)
    {
        return attitude.eulerAngles.y;
    }

    private void UpdateTilt()
    {
        Quaternion attitude = Attitude();

        transform.rotation = attitude;
        transform.Rotate(-90, 0, 0);

        XRotText.text = "X Rot: " + transform.rotation.eulerAngles.x;
        YRotText.text = "Y Rot: " + transform.rotation.eulerAngles.y;
        ZRotText.text = "Z Rot: " + transform.rotation.eulerAngles.z;
    }
}