using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public float timeBetweenWindupSoundAndTelekinesis;
    [SerializeField] Sound[] sounds;
    [SerializeField] bool debug;

    public static AudioManager instance;

    void Awake()
    {
        // Wenn schon ein Audiomanager Existiert, mach ihn kaputt
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
            if (item.startOnPlay)
            {
                Play(item.name);
                item.isPlaying = true;
            }
        }
    }


    public float GetLength(string clipName)
    {

        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning("AudioScript Error: Dessen Length zu gettender Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return 0;
        }

        return soundToPlay.myClip.length;
    }

    public void Play(string clipName)
    {
        if (debug)
            print(clipName + " should play");

        Sound soundToPlay = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToPlay == null)
        {
            Debug.LogWarning($"AudioScript Error: Der zu spielende Clipname -> {clipName} <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToPlay.randomPitch)
        {
            float randomPitch = UnityEngine.Random.Range(soundToPlay.randomPichBetweenTwoValues.x, soundToPlay.randomPichBetweenTwoValues.y);
            soundToPlay.mySource.pitch = randomPitch;
        }

        if (soundToPlay.fadeCoroutine != null)
        {
            StopCoroutine(soundToPlay.fadeCoroutine);
            soundToPlay.fadeCoroutine = null;
        }

        if(!soundToPlay.isPlaying)
        {
            soundToPlay.mySource.Play();
        }
            

        if(soundToPlay.myFadeIn != 0)
        { 
            soundToPlay.fadeCoroutine = StartCoroutine(Fade(soundToPlay, soundToPlay.currentVolume, soundToPlay.myVolume, soundToPlay.myFadeIn));
            soundToPlay.isPlaying = true;
        }
               
    }

    public void Stop(string clipName)
    {
        if (debug)
            print(clipName + " should stop");

        Sound soundToStop = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToStop == null)
        {
            Debug.LogWarning($"AudioScript Error: Der zu stoppende Clipname -> {clipName} <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToStop.fadeCoroutine != null)
        {
            StopCoroutine(soundToStop.fadeCoroutine);
            soundToStop.fadeCoroutine = null;
        }

        if (soundToStop.myFadeOut == 0)
            soundToStop.mySource.Stop();
        else soundToStop.fadeCoroutine = StartCoroutine(Fade(soundToStop, soundToStop.currentVolume, 0, soundToStop.myFadeOut));

    }

    IEnumerator Fade(Sound sound, float startVolume, float endVolume, float fadeTime)
    {
        float elapsedTime = 0f;
        AudioSource audioSource = sound.mySource;

        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            sound.currentVolume = audioSource.volume;

            yield return null;
        }

        audioSource.volume = endVolume;

        if (audioSource.volume == 0f)
        {
            audioSource.Stop();
            sound.isPlaying = false;
        }
    }


}

