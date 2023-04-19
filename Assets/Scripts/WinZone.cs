using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    [SerializeField] float stayTimeToWin = 5;
    [SerializeField] bool debug;
    GameObject winObject;
    float winTimer;
    bool timerRun;

    LevelManager myManager;

    private void Start()
    {
        myManager = FindObjectOfType<LevelManager>();
        winTimer = stayTimeToWin;
        timerRun = false;
    }

    private void Update()
    {
        WintimerProgress();
    }

    private void OnTriggerStay(Collider other)
    {
        if (winObject == null && other.tag != "Attached") // Wenn ein Objekt mit !Attached.tag in der Zone erscheint - Starte den Win-Timer-Stuff
        {
            AttachWinObject(other.gameObject);
        }
        else if (winObject != null && other.tag == "Attached") // Wenn das Eingeloggte Win Objekt attached wird aber noch in der TriggerZone chillt
        {
            DetachWinObject();

            if (debug)
            {
                Debug.Log("Win object was Attached by player!");
                Debug.Log("Win Timer wurde resettet!");
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (winObject == null) // Wenn kein WInobjekt eingeloggt ist - spars dir 
            return;

        if (other.gameObject == winObject) // Wenn das eingeloggte WIn Objekt die Win Zone verlässt, logg es aus und setze den Timer zurück!
        {
            DetachWinObject();

            if (debug)
            {
                Debug.Log("Win object left Winzone!");
                Debug.Log("Win Timer wurde resettet!");
            }
        }
    }

    void WintimerProgress()
    {
        if (timerRun)
        {
            winTimer -= Time.deltaTime;
            if (winTimer <= 0)
            {
                // WIN!!!!!!!!!!!!!!
                OnWin();

                if (debug)
                    Debug.Log("WON!!!");

            }
        }
    }

    void AttachWinObject(GameObject newWinObject)
    {
        timerRun = true;
        winObject = newWinObject;

        if (debug)
            Debug.Log("Win object entered Winzone!");
    }

    void DetachWinObject()
    {
        winObject = null;
        timerRun = false;
        winTimer = stayTimeToWin;

    }

    void OnWin()
    {
        myManager.LevelWon();

        this.enabled = false;
    }
}
