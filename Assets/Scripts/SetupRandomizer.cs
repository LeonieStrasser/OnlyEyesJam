using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] allSceneSets;
    [SerializeField] GameObject warmColorParticles;
    [SerializeField] GameObject coldColorParticles;


    ColorChanger myColorChanger;

    private void Awake()
    {
        myColorChanger = GetComponentInChildren<ColorChanger>();
    }

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
            warmColorParticles.SetActive(true);
            coldColorParticles.SetActive(false);
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
