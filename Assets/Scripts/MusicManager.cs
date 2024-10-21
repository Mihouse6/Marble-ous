using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private float NextTrackDelay;
    private float NextTrackTimer;

    private void Start()
    {
        NextTrackTimer = Time.time;
    }

    private void Update()
    {
        if (Time.time >= NextTrackTimer)
            PlayRandomTrack();
    }

    //Selects a random track to be played, and calculates the start time of the next track
    private void PlayRandomTrack()
    {
        AudioClip[] tracks = AudioManager.instance.MusicClips;
        AudioClip nextTrack = tracks[Random.Range(0, tracks.Length)];

        AudioManager.instance.PlayMusic(nextTrack.name);
        NextTrackTimer = Time.time + nextTrack.length + NextTrackDelay;
    }
}
