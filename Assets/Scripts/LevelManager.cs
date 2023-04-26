using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] GameObject winObject;

    [SerializeField] bool debug;

    [SerializeField] List<WinZone> allWinZones;
    int succededWinzones;

    enum levelState
    {
        play,
        pause,
        won
    }

    levelState currentLevelState;

    private void Awake()
    {
        instance = this;
        allWinZones = new List<WinZone>();
        WinZone[] sceneWinZones = FindObjectsOfType<WinZone>();
        foreach (var item in sceneWinZones)
        {
            allWinZones.Add(item);
        }
    }

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


    // WINZONES

    public void ReportSuccededWinZone()
    {
        succededWinzones++;

        if(succededWinzones == allWinZones.Count)
        {
            // Level gewonnen!
            foreach (var item in allWinZones)
            {
                item.SetWin();
            }
        }
        else if(succededWinzones > allWinZones.Count)
        {
            Debug.LogWarning("Es sind mehr Winzones als -Done- eingeloggt als im Manager angemeldet sind!", gameObject);
        }
    }
    public void ReportLostWinZone()
    {
        succededWinzones--;
    }
}
