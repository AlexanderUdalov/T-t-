using System;
using System.Collections;
using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private const float FadeTime = 1f;
    public AudioClip Intro, Menu;
    public AudioClip[] Tracks;

    private void Awake()
    {
        EazySoundManager.PlayMusicDelayed(Menu, Intro.length);
    }

    private void Start() 
    {
        EazySoundManager.PlayMusic(Intro);
    }

    public void SwitchToMenu()
    {
        EazySoundManager.PlayMusic(Menu);
    }

    public void SwitchToTracks()
    {
        var random = new System.Random();
        EazySoundManager.StopAllMusic(1f);
        EazySoundManager.PlayMusic(Tracks[random.Next(Tracks.Length - 1)]);
    }
}
