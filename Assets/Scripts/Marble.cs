using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] private float CollisionVelocityThreshold;
    [SerializeField] private float MaxCollisionVelocityThreshold;
    //[SerializeField] private float CollisionIncidenceThreshold;
    [SerializeField] private AudioSource CollisionSound;
    [SerializeField] private float CollisionCooldown;
    private float CollisionTimer;

    private void Start()
    {
        CollisionTimer = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Fail conitions
        if (!collision.collider.CompareTag("Wall")) return;
        if (Time.time < CollisionTimer) return;
        if (collision.relativeVelocity.magnitude < CollisionVelocityThreshold) return;
        
        //Play sound & vibration
        float maxVol = AudioManager.instance.MaxSFXVolume;
        float midVol = maxVol / 2f;
        CollisionSound.volume = (Mathf.Lerp(CollisionVelocityThreshold, MaxCollisionVelocityThreshold, Mathf.Min(collision.relativeVelocity.magnitude, MaxCollisionVelocityThreshold)) * midVol) + midVol;
        CollisionSound.Play();
        HapticsManager.instance.MediumVibration();

        //Reset collision cooldown timer
        CollisionTimer = Time.time + CollisionCooldown;
    }
}
