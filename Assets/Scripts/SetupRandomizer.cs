using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRandomizer : MonoBehaviour
{
    ColorChanger myColorChanger;

    private void Awake()
    {
        myColorChanger = GetComponentInChildren<ColorChanger>();
    }

    public void RandomizeSetup()
    {

    }
}
