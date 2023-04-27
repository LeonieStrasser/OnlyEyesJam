using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class MatchGazePosition : MonoBehaviour
{
    Camera mainCam;

    Vector3 gazeWorldPos;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        gazeWorldPos = mainCam.ScreenToWorldPoint(TobiiAPI.GetGazePoint().Screen);
        
        transform.position = new Vector3(gazeWorldPos.x, gazeWorldPos.y, 0f);
    }
}
