using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] GameObject winObject;

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

        winObject.SetActive(true);

        if (debug)
            Debug.Log("WON!!!!");

    }

    public void RelodeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
