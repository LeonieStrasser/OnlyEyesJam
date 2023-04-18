using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] bool debug;

    enum levelState
    {
        play,
        pause,
        won
    }

    levelState currentLevelState;

    private void Start()
    {
        currentLevelState = levelState.play;
    }

    public void LevelWon()
    {
        currentLevelState = levelState.won;

        if (debug)
            Debug.Log("WON!!!!");

    }
}
