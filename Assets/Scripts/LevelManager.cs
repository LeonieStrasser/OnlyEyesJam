using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    ConductorManager myConductorManager;

    [SerializeField] GameObject winObject;
    [SerializeField] Animator sceneTransitionAnim;

    [SerializeField] bool debug;

    public List<WinZone> allWinZones;
    [HideInInspector] public int attachedWinzones;
    int succededWinzones;

    enum levelState
    {
        play,
        pause,
        won
    }

    levelState currentLevelState;

    ObjectSpawner spawner;

    SetupRandomizer randomizer;

    CubeGroupFeedback myWinFeedback;

    void Awake()
    {
        instance = this;
        allWinZones = new List<WinZone>();
        myConductorManager = FindObjectOfType<ConductorManager>();
    }

    void Start()
    {
        currentLevelState = levelState.play;
        myWinFeedback = FindObjectOfType<CubeGroupFeedback>();
        spawner = GetComponent<ObjectSpawner>();
        randomizer = FindObjectOfType<SetupRandomizer>();
        randomizer.RandomizeSetup();
    }

    public void RegisterWinZones()
    {
        WinZone[] sceneWinZones = FindObjectsOfType<WinZone>();
        foreach (var item in sceneWinZones)
        {
            allWinZones.Add(item);
        }
    }

    public void LevelWon()
    {
        currentLevelState = levelState.won;
        //winObject.SetActive(true);
        
        //Cube Gruppe Feedback
        GazeManager.Instance.SetGazeActive(false);

        // Setze den World Border Effeki in Gang
        myWinFeedback.StartWingroupsEffect();
        // Sag den Cubes bescheid

        List<ConductorList> _allWingroups = new List<ConductorList>();
        foreach (var item in allWinZones) // Geh alle WInzones durch
        {
            ConductorList _winGroup;
            Conductor _wingroupConductor = item.WinObject.GetComponent<Conductor>(); // Finde heraus welcher Cube in der Winzone liegt
            myConductorManager.CheckConnectionGroupsForConductor(_wingroupConductor, out _winGroup); // Finde raus zu welchjer ConductorGruppe er geh�rt
            if (_winGroup == null) // Wenn er zu keiner geh�rt also allein ist, kreiere eine neue Liste in der nur er ist
            {
                ConductorList _newList = new ConductorList();
                _newList.Initialize();
                _newList.allCunductors.Add(_wingroupConductor);
                _allWingroups.Add(_newList);
            }
            else if (!_allWingroups.Contains(_winGroup)) // Wenn er in einer Gruppe liegt und diese noch nicht in der Winning groups liste ist, f�ge die gruppe hinzu
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
                    _conductor.gameObject.GetComponent<ObjectState>().winGroup = true;
                }
            }
        }
        else
        {
            Debug.LogError("spiel ist gewonnen aber es gibt keine Gruppe die im Ziel steckt --- WTF!???", gameObject);
        }

        if (debug)
            Debug.Log("WON!!!!");

        attachedWinzones = 0;
        
        StartSceneTransition();
    }

    public void RelodeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void StartSceneTransition()
    {
        if(debug)
            Debug.Log("SceneTransitionStarted");
        
        StartCoroutine(SceneTransition());
    }

    IEnumerator SceneTransition()
    {
        yield return new WaitForSeconds(9.75f);
        
        // Fade-In
        sceneTransitionAnim.SetTrigger(Animator.StringToHash("fadeIn"));

        yield return new WaitForSeconds(2.5f);
        
        myConductorManager.ClearConductorList();
        spawner.ClearAllObjects();
        ClearWinZones();
        succededWinzones = 0;
        
        AudioManager.instance.Stop("Win Wind");
        AudioManager.instance.Stop("Win Leaves");
        
        randomizer.RandomizeSetup();
        
        // Fade-Out
        sceneTransitionAnim.SetTrigger(Animator.StringToHash("fadeOut"));
        AudioManager.instance.Stop("Win Static");
        
        yield return new WaitForSeconds(1f);
        
        GazeManager.Instance.SetGazeActive(true);
        
        spawner.StartSpawning();
        
        yield return new WaitForSeconds(4.5f);
        
        spawner.PlaceWinZones();
    }
    
    void ClearWinZones()
    {
        for (int i = allWinZones.Count - 1; i >= 0; i--)
        {
            Destroy(allWinZones[i].gameObject);
        }
        
        allWinZones.Clear();
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
            LevelWon();
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
