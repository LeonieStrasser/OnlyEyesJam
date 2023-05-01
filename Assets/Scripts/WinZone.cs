using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WinZone : MonoBehaviour
{
    [SerializeField] float stayTimeToWin = 5;
    [SerializeField] bool debug;
    GameObject winObject;
    public GameObject WinObject { get { return winObject; } }
    bool attachTriggered;
    bool winzoneSucceeded;
    float winTimer;
    float winningProgress; // timeProgress zwischen 0 und 1
    bool timerRun;

    LevelManager myManager;

    // FEEDBACK
    [SerializeField] MeshRenderer meshRen;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnDetach;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnEnter;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnAttachTrigger;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnWin;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem winVFX;

    void Start()
    {
        myManager = FindObjectOfType<LevelManager>();
        winTimer = stayTimeToWin;
        timerRun = false;
    }

    void Update()
    {
        WintimerProgress();
    }

    void OnTriggerStay(Collider other)
    {
        if (!(other.CompareTag("MoveableObject")) && !(other.CompareTag("Attached"))) // Wenn es kein Hühnergott ist, dann ists eh egal!
            return;
        else if (other.CompareTag("Attached") && !attachTriggered) // Wenn es Hühnergott ist der vom Player grad bewegt wird - Triggerfeedback
        {
            AttachTriggerFeedback();
            attachTriggered = true;
        }


        if (winObject == null && other.tag != "Attached") // Wenn ein Objekt mit !Attached.tag in der Zone erscheint - Starte den Win-Timer-Stuff
        {
            attachTriggered = false;
            AttachWinObject(other.gameObject);
        }
        else if (winObject != null && other.tag == "Attached") // Wenn das Eingeloggte Win Objekt attached wird aber noch in der TriggerZone chillt
        {
            DetachWinObject();
            AttachTriggerFeedback();
            attachTriggered = true;
            if (debug)
            {
                Debug.Log("Win object was Attached by player!");
                Debug.Log("Win Timer wurde resettet!");
            }
        }

    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MoveableObject") || (other.CompareTag("Attached")))
        {

            attachTriggered = false;

            if (winObject == null) // Wenn kein WInobjekt eingeloggt ist verlässt ein attchter Block die Winzone
            {
                DetachFeedback();
                return;
            }
            else if (other.gameObject == winObject) // Wenn das eingeloggte WIn Objekt die Win Zone verl�sst, logg es aus und setze den Timer zur�ck!
            {

                DetachWinObject();

                if (debug)
                {
                    Debug.Log("Win object left Winzone!");
                    Debug.Log("Win Timer wurde resettet!");
                }
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
                // MELDE DEM LEVELMANAGER EINEN ERFOOOLG!!! JUHUUU!
                if (!winzoneSucceeded)
                    OnSucceed();

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

        EnterFeedback();

        if (debug)
            Debug.Log("Win object entered Winzone!");
    }

    void DetachWinObject()
    {
        winObject = null;
        timerRun = false;
        winTimer = stayTimeToWin;

        if (winzoneSucceeded)
        {
            winzoneSucceeded = false;
            LevelManager.instance.ReportLostWinZone();
        }
        
        DetachFeedback();
    }

    void OnSucceed()
    {
        winzoneSucceeded = true;
        LevelManager.instance.ReportSuccededWinZone();
    }

    public void SetWin()
    {
        //winObject.GetComponent<ObjectState>().ChangePhysicalState(ObjectState.physicalStates.Grounded);

        WinFeedback();
    }
    
    #region feedback
    void AttachTriggerFeedback()
    {
        meshRen.material.SetColor(Shader.PropertyToID("_Color"), colorOnAttachTrigger);
    }

    void EnterFeedback()
    {
        meshRen.material.SetColor(Shader.PropertyToID("_Color"), colorOnEnter);
        Debug.Log("Enter Feedback!");
    }

    void DetachFeedback()
    {
        meshRen.material.SetColor(Shader.PropertyToID("_Color"), colorOnDetach);
    }

    void AttatchedProgressFeedback() // W�hrend der Win timer hochz�hlt
    {
        Color lerpedColor = Color.Lerp(colorOnWin, colorOnEnter, winningProgress);
        meshRen.material.SetColor(Shader.PropertyToID("_Color"), lerpedColor);
    }

    void WinFeedback()
    {
        //winVFX.transform.SetParent(null);
        winVFX.Play();
    }

    #endregion
}
