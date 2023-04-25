using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WinZone : MonoBehaviour
{
    [SerializeField] float stayTimeToWin = 5;
    [SerializeField] bool debug;
    GameObject winObject;
    float winTimer;
    float winningProgress; // timeProgress zwischen 0 und 1
    bool timerRun;

    LevelManager myManager;
    
    // FEEDBACK
    MeshRenderer myRenderer;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnDetach;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnAttach;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnWin;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem winVFX;
    
    void Start()
    {
        myManager = FindObjectOfType<LevelManager>();
        winTimer = stayTimeToWin;
        timerRun = false;

        myRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        WintimerProgress();
    }

    void OnTriggerStay(Collider other)
    {
        if(!(other.CompareTag("MoveableObject") || other.CompareTag("Attached")))
            return;
        
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

        if (other.gameObject == winObject) // Wenn das eingeloggte WIn Objekt die Win Zone verl�sst, logg es aus und setze den Timer zur�ck!
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
            winningProgress = winTimer / stayTimeToWin;

            AttatchedProgressFeedback();

            if (winTimer <= 0)
            {
                // WIN!!!!!!!!!!!!!!
                OnWin();

                if (debug)
                    Debug.Log("WON!!!");
            }
        }
    }

    void AttachWinObject(GameObject _newWinObject)
    {
        timerRun = true;
        winObject = _newWinObject;
        
        winObject.GetComponent<ObjectState>().ChangePhysicalState(ObjectState.physicalStates.Immovable);

        AttachFeedback();

        if (debug)
            Debug.Log("Win object entered Winzone!");
    }

    void DetachWinObject()
    {
        winObject = null;
        timerRun = false;
        winTimer = stayTimeToWin;

        DetachFeedback();
    }

    void OnWin()
    {
        //winObject.GetComponent<ObjectState>().ChangePhysicalState(ObjectState.physicalStates.Grounded);
        
        myManager.LevelWon();
        WinFeedback();

        this.enabled = false;
    }


    #region feedback

    void AttachFeedback()
    {
        myRenderer.material.color = colorOnAttach;
    }

    void DetachFeedback()
    {
        myRenderer.material.color = colorOnDetach;
    }

    void AttatchedProgressFeedback() // W�hrend der Win timer hochz�hlt
    {
        Color lerpedColor = Color.Lerp(colorOnWin, colorOnAttach, winningProgress);
        myRenderer.material.color = lerpedColor;
    }

    void WinFeedback()
    {
        winVFX.Play();
    }

    #endregion
}
