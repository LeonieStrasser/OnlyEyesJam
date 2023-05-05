using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Random = UnityEngine.Random;

public class SetupRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] allSceneSets;
    [SerializeField] GameObject warmColorParticles;
    [SerializeField] GameObject coldColorParticles;


    ColorChanger myColorChanger;

    void Awake()
    {
        myColorChanger = GetComponentInChildren<ColorChanger>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
            RandomizeSetup();
    }

    [Button]
    public void RandomizeSetup()
    {
        SetEnvironmentArrangement();
        SetColorSetup();
    }

    void SetColorSetup()
    {
        bool _warmColors;
        myColorChanger.SetRandomColorsInScene(out _warmColors);

        if (_warmColors)
        {
            warmColorParticles.SetActive(true);
            coldColorParticles.SetActive(false);
        }
        else
        {
            warmColorParticles.SetActive(false);
            coldColorParticles.SetActive(true);
        }
    }

    void SetEnvironmentArrangement()
    {
        int _randomListIndex = Random.Range(0, allSceneSets.Length);

        foreach (var item in allSceneSets)
        {
            item.SetActive(false);
        }
        allSceneSets[_randomListIndex].SetActive(true);
    }
}
