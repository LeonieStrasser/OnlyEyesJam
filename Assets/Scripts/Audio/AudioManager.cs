/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public float timeBetweenWindupSoundAndTelekinesis;
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
            Debug.LogWarning("AudioScript Error: Der zu spielende Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToPlay.randomPitch)
        {
            float randomPitch = UnityEngine.Random.Range(soundToPlay.randomPichBetweenTwoValues.x, soundToPlay.randomPichBetweenTwoValues.y);
            soundToPlay.mySource.pitch = randomPitch;
        }

        if (soundToPlay.fadeInCoroutine != null)
        {
            StopCoroutine(soundToPlay.fadeInCoroutine);
        }

        //soundToPlay.mySource.volume = 0f;
        soundToPlay.mySource.Play();

        soundToPlay.fadeInCoroutine = StartCoroutine(FadeIn(soundToPlay));
    }

    private Coroutine lastFadeInRoutine, lastFadeOutRoutine;

    IEnumerator FadeIn(Sound soundToFadeIn)
    {
        
        float elapsedTime = 0f;
        float fadeTime = soundToFadeIn.myFadeIn;
        AudioSource audioSource = soundToFadeIn.mySource;
        float lastVolume = soundToFadeIn.mySource.volume;
        float targetVolume = soundToFadeIn.myVolume;

        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(lastVolume, targetVolume, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = soundToFadeIn.myVolume;
    }

    public void Stop(string clipName)
    {
        Sound soundToStop = Array.Find(sounds, sound => sound.name == clipName);

        if (soundToStop == null)
        {
            Debug.LogWarning("AudioScript Error: Der zu stoppende Clipname -> " + clipName + " <- kann in der Audio Liste nicht gefunden werden! Habt ihr ihn falsch geschrieben ihr Pappnasen???");
            return;
        }

        if (soundToStop.fadeInCoroutine != null)
        {
            StopCoroutine(soundToStop.fadeInCoroutine);
            soundToStop.fadeInCoroutine = null;
        }

        StartCoroutine(FadeOut(soundToStop));
    }

    IEnumerator FadeOut(Sound soundToFadeOut)
    {
        float elapsedTime = 0f;
        float lastVolume = soundToFadeOut.mySource.volume;
        float fadeTime = soundToFadeOut.myFadeOut;
        AudioSource audioSource = soundToFadeOut.mySource;

        while (elapsedTime < fadeTime)
        {
            audioSource.volume = Mathf.Lerp(lastVolume, 0f, elapsedTime / fadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
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

    void RandomizePitch()
    {

    }
}
*/

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
            }

            item.CustomStart();
        }
    }

    private void Update()
    {
        foreach (var item in sounds)
        {
            item.CustomUpdate();
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

        if (soundToPlay.fadeInCoroutine != null)
        {
            StopCoroutine(soundToPlay.fadeInCoroutine);
        }

        soundToPlay.mySource.Play();

        soundToPlay.SetFader(soundToPlay.mySource.volume, soundToPlay.myVolume, soundToPlay.myFadeIn);
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

        if (soundToStop.fadeInCoroutine != null)
        {
            StopCoroutine(soundToStop.fadeInCoroutine);
            soundToStop.fadeInCoroutine = null;
        }

        if (soundToStop.fadeOutCoroutine != null)
        {
            StopCoroutine(soundToStop.fadeOutCoroutine);
            soundToStop.fadeOutCoroutine = null;
        }

        soundToStop.SetFader(soundToStop.mySource.volume, 0f, soundToStop.myFadeOut);
    }


}

AudioManager.cs
9 KB