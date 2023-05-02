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
    public Coroutine fadeInCoroutine, fadeOutCoroutine;

    [HideInInspector] public float startVolume, endVolume, elapsedTime, fadeTime;

    public void CustomStart()
    {
        Debug.Log("custom start");
        startVolume = endVolume = myVolume;
        fadeTime = 1;
    }
    public void CustomUpdate()
    {
        mySource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeTime);
        elapsedTime += Time.deltaTime;

        if (mySource.volume == 0f)
        {
            mySource.Stop();
        }
    }

    public void SetFader(float _startVolume, float _endVolume, float _fadeTime)
    {
        startVolume = _startVolume;
        endVolume = _endVolume;
        fadeTime = _fadeTime;
        elapsedTime = 0;
    }
}

