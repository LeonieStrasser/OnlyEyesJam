using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    ConductorManager myConductorManager;

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

    CubeGroupFeedback myWinFeedback;

    private void Awake()
    {
        instance = this;
        allWinZones = new List<WinZone>();
        myConductorManager = FindObjectOfType<ConductorManager>();
        WinZone[] sceneWinZones = FindObjectsOfType<WinZone>();
        foreach (var item in sceneWinZones)
        {
            allWinZones.Add(item);
        }
    }

    private void Start()
    {
        currentLevelState = levelState.play;
        myWinFeedback = FindObjectOfType<CubeGroupFeedback>();

    }

    public void LevelWon()
    {
        currentLevelState = levelState.won;
        winObject.SetActive(true);

        //Cube Gruppe Feedback
        List<ConductorList> _allWingroups = new List<ConductorList>();
        foreach (var item in allWinZones) // Geh alle WInzones durch
        {
            ConductorList _winGroup;
            Conductor _wingroupConductor = item.WinObject.GetComponent<Conductor>(); // Finde heraus welcher Cube in der Winzone liegt
            myConductorManager.CheckConnectionGroupsForConductor(_wingroupConductor, out _winGroup); // Finde raus zu welchjer ConductorGruppe er gehört
            if (_winGroup == null) // Wenn er zu keiner gehört also allein ist, kreiere eine neue Liste in der nur er ist
            {
                ConductorList _newList = new ConductorList();
                _newList.Initialize();
                _newList.allCunductors.Add(_wingroupConductor);
                _allWingroups.Add(_newList);
            }
            else if (!_allWingroups.Contains(_winGroup)) // Wenn er in einer Gruppe liegt und diese noch nicht in der Winning groups liste ist, füge die gruppe hinzu
            {
                _allWingroups.Add(_winGroup);
            }
        }

        if (_allWingroups.Count > 0)
        {
            // Alle wingroups anschalten
            foreach (var item in _allWingroups)
            {
                foreach (var _conductor in item.allCunductors)
                {
                    _conductor.gameObject.GetComponent<ObjectState>().ChangeVisualState(ObjectState.visualStates.WinGroup);
                }
            }

        }
        else
        {
            Debug.LogError("spiel ist gewonnen aber es gibt keine Gruppe die im Ziel steckt --- WTF!???", gameObject);
        }

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

        if (succededWinzones == allWinZones.Count)
        {
            // Level gewonnen!
            foreach (var item in allWinZones)
            {
                item.SetWin();
            }
        }
        else if (succededWinzones > allWinZones.Count)
        {
            Debug.LogWarning("Es sind mehr Winzones als -Done- eingeloggt als im Manager angemeldet sind!", gameObject);
        }
    }
    public void ReportLostWinZone()
    {
        succededWinzones--;
    }
}
