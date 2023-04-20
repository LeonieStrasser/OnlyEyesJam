using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {

        // Wenn schon ein Audiomanager Existiert, mach ihn kaputt
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (var item in sounds)
        {
            item.mySource = gameObject.AddComponent<AudioSource>();
            item.mySource.clip = item.myClip;
            item.mySource.volume = item.myVolume;
            item.mySource.pitch = item.myPitch;
            item.mySource.loop = item.myLoop;
        }
    }

    private void Start()
    {
        foreach (var item in sounds)
        {
            if(item.startOnPlay)
            {
                Play(item.name);
            }
        }
    }


    public void Play(string clipName)
    {
        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Der Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if(soundToPlay.randomPitch)
        {
            float randomPitch = UnityEngine.Random.Range(soundToPlay.randomPichBetweenTwoValues.x, soundToPlay.randomPichBetweenTwoValues.y);
            soundToPlay.mySource.pitch = randomPitch;
        }

        soundToPlay.mySource.Play();
    }


    void RandomizePitch()
    {

    }
}
