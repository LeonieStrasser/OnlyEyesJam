using UnityEngine;
using NaughtyAttributes;


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

    [ShowIf("randomPitch")] [MinMaxSlider(.1f, 3f)] public Vector2 randomPichBetweenTwoValues;

    [HideInInspector]
    public AudioSource mySource;
}
