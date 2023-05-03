using UnityEngine;
using NaughtyAttributes;
using System;


[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip myClip;

    [Range(0f, 1f)]
    public float myVolume;
    [Range(.1f, 3f)]
    public float myPitch;
    public bool myLoop;
    public bool startOnPlay;
    public bool randomPitch;
    [Range(0f, 4f)]
    public float myFadeIn;
    [Range(0f, 4f)]
    public float myFadeOut;

    [ShowIf("randomPitch")] [MinMaxSlider(.1f, 3f)] public Vector2 randomPichBetweenTwoValues;

    [HideInInspector]
    public AudioSource mySource;

    [HideInInspector]
    public Coroutine fadeCoroutine;

    [HideInInspector] 
    public float currentVolume;

    [HideInInspector]
    public bool isPlaying = false;


}

