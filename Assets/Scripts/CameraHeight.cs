using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeight : MonoBehaviour
{
    [SerializeField] private float PadPercent;

    private void Start()
    {
        //Calculate the target frustrum height
        float frustrumHeight = (FindObjectOfType<Grid>().DimensionY + 1f) * (1f + PadPercent);

        //Reverse calculate the distance that the camera needs to be from the board, and set it
        Vector3 localPos = transform.localPosition;
        localPos.y = frustrumHeight * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        transform.localPosition = localPos;
    }
}
