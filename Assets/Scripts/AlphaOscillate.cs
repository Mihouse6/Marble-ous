using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaOscillate : MonoBehaviour
{
    [SerializeField] private float MinTransparency;
    [SerializeField] private float MaxTransparency;
    [SerializeField] private float OscillationPeriod;

    private void Update()
    {
        Material material = GetComponent<MeshRenderer>().material;
        Color colour = material.color;
        float value = Mathf.Lerp(MinTransparency, MaxTransparency, (Mathf.Cos(Time.time * 2f * Mathf.PI / OscillationPeriod) + 1f) / 2f);
        Debug.Log(value);
        colour.a = value;
        material.color = colour;
    }
}
