using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;
using Random = System.Random;

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

    Animator anim;

    LevelManager myManager;

    Material mat;

    // FEEDBACK
    [SerializeField] MeshRenderer meshRen;
    [BoxGroup("Feedback")] [SerializeField] Color idleColor;
    [BoxGroup("Feedback")] [SerializeField] float idleDeformStrength;

    [BoxGroup("Feedback")] [SerializeField] Color colorOnEnter;
    [BoxGroup("Feedback")] [SerializeField] Color colorOnAttachTrigger;

    [BoxGroup("Feedback")] [SerializeField] Color colorOnWin;
    [BoxGroup("Feedback")] [SerializeField] float winningDeformStrength;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem winVFX;

    void Start()
    {
        myManager = FindObjectOfType<LevelManager>();
        anim = GetComponent<Animator>();

        mat = meshRen.material;
        
        winTimer = stayTimeToWin;
        timerRun = false;
        
        DetachFeedback();
    }

    void Update()
    {
        WintimerProgress();
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("MoveableObject") && !other.CompareTag("Attached")) // Wenn es kein Hühnergott ist, dann ists eh egal!
            return;
        
        if (other.CompareTag("Attached") && !attachTriggered) // Wenn es Hühnergott ist der vom Player grad bewegt wird - Triggerfeedback
        {
            AttachTriggerFeedback();
            attachTriggered = true;
        }
        
        if (winObject == null && !other.CompareTag("Attached")) // Wenn ein Objekt mit !Attached.tag in der Zone erscheint - Starte den Win-Timer-Stuff
        {
            attachTriggered = false;
            AttachWinObject(other.gameObject);
        }
        
        else if (winObject != null && other.CompareTag("Attached")) // Wenn das Eingeloggte Win Objekt attached wird aber noch in der TriggerZone chillt
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
        if (other.CompareTag("MoveableObject") || other.CompareTag("Attached"))
        {
            attachTriggered = false;

            if (winObject == null) // Wenn kein Winobjekt eingeloggt ist verlässt ein attatchter Block die Winzone
            {
                DetachFeedback();
            }
            
            else if (other.gameObject == winObject) // Wenn das eingeloggte Win Objekt die Win Zone verl�sst, logg es aus und setze den Timer zur�ck!
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
            winningProgress = 1 - Mathf.Clamp(winTimer / stayTimeToWin, 0, 1);

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
        mat.SetColor(Shader.PropertyToID("_Color"), colorOnAttachTrigger);

        anim.SetTrigger(Animator.StringToHash("contactWobble"));
    }

    void EnterFeedback()
    {
        mat.SetColor(Shader.PropertyToID("_Color"), colorOnEnter);
        anim.SetTrigger(Animator.StringToHash("contactWobble"));
        Debug.Log("Enter Feedback!");
    }

    void DetachFeedback()
    {
        mat.SetColor(Shader.PropertyToID("_Color"), idleColor);
        
        mat.SetFloat(Shader.PropertyToID("_DeformStrength"), idleDeformStrength);
    }

    void AttatchedProgressFeedback() // W�hrend der Win timer hochz�hlt
    {
        Color lerpedColor = Color.Lerp(colorOnEnter, colorOnWin, winningProgress);
        mat.SetColor(Shader.PropertyToID("_Color"), lerpedColor);
        
        mat.SetFloat(Shader.PropertyToID("_DeformStrength"), Mathf.Lerp(idleDeformStrength, winningDeformStrength, winningProgress));
    }

    void WinFeedback()
    {
        //winVFX.transform.SetParent(null);
        winVFX.Play();
    }

    #endregion
}
