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

    // FEEDBACK
    [SerializeField] MeshRenderer meshRen;

    [BoxGroup("Feedback")] [SerializeField] float idleDeformStrength;
    [BoxGroup("Feedback")] [SerializeField] float winningDeformStrength;

    [BoxGroup("Feedback")] [SerializeField] ParticleSystem sparkleVFX;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem winWaveVFX;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem sparkleLeavesVFX;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem winDissolveStart;
    [BoxGroup("Feedback")] [SerializeField] ParticleSystem[] winVFX;

    [BoxGroup("Feedback")] [SerializeField] AnimationCurve wobbleCurve;
    [BoxGroup("Feedback")] [SerializeField] float wobbleStrength;
    [BoxGroup("Feedback")] [SerializeField] float wobbleDuration;

    bool enabled = true;

    Material mat;

    GameObject winObject;
    public GameObject WinObject { get { return winObject; } }

    bool attachTriggered;
    bool winzoneSucceeded;
    float winTimer;
    float winningProgress; // timeProgress zwischen 0 und 1
    bool timerRun;

    Color idleColor, winColor;
    bool wobbleAnimationRunning, idleTransitionRunning;

    void Start()
    {
        mat = meshRen.material;

        idleColor = mat.GetColor(Shader.PropertyToID("_IdleColor"));
        winColor = mat.GetColor(Shader.PropertyToID("_WinColor"));

        winTimer = stayTimeToWin;
        timerRun = false;

        mat.SetColor(Shader.PropertyToID("_CurrentColor"), idleColor);
    }

    void Update()
    {
        if(!enabled)
            return;
        
        WintimerProgress();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MoveableObject") || other.CompareTag("Attached"))
        {
            // Layeränderung u FrontRender
            //other.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer("FrontRender");
            other.GetComponent<ObjectState>().WinZoneEnter();
        }
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
            // Layeränderung zu default
            //other.gameObject.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Default");
            
            other.GetComponent<ObjectState>().WinZoneExit();
            
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
        LevelManager.instance.attachedWinzones++;

        //winObject.GetComponent<ObjectState>().ChangePhysicalState(ObjectState.physicalStates.Immovable);

        EnterFeedback();

        if (debug)
            Debug.Log("Win object entered Winzone!");
    }

    void DetachWinObject()
    {
        winObject = null;
        timerRun = false;
        winTimer = stayTimeToWin;
        LevelManager.instance.attachedWinzones--;

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
        winObject.GetComponent<ObjectState>().ChangePhysicalState(ObjectState.physicalStates.Grounded);

        enabled = false;
        
        WinFeedback();
    }

    #region feedback
    void AttachTriggerFeedback()
    {
        if (!wobbleAnimationRunning)
            StartCoroutine(WobbleAnimation());
    }

    void EnterFeedback()
    {
        if (!wobbleAnimationRunning)
            StartCoroutine(WobbleAnimation());

        //sparkleVFX.Play();
        sparkleLeavesVFX.Play();

        if (LevelManager.instance.attachedWinzones == LevelManager.instance.allWinZones.Count)
            if (!winzoneSucceeded)
                AudioManager.instance.Play("Win Static");
    }

    void DetachFeedback()
    {
        StartCoroutine(IdleTransition());

        //sparkleVFX.Stop();
        sparkleLeavesVFX.Stop();

        //if (LevelManager.instance.attachedWinzones < 1)
            AudioManager.instance.Stop("Win Static");
    }

    void AttatchedProgressFeedback() // W�hrend der Win timer hochz�hlt
    {
        Color lerpedColor = Color.Lerp(idleColor, winColor, winningProgress);

        mat.SetColor(Shader.PropertyToID("_CurrentColor"), lerpedColor);

        mat.SetFloat(Shader.PropertyToID("_DeformStrength"), Mathf.Lerp(idleDeformStrength, winningDeformStrength, Mathf.Clamp(winningProgress * 2f, 0, 1)));
    }

    IEnumerator WobbleAnimation()
    {
        wobbleAnimationRunning = true;

        float animationTime = 0;

        while (animationTime < wobbleDuration)
        {
            float deformStrength = Mathf.Lerp(idleDeformStrength, wobbleStrength,
                wobbleCurve.Evaluate(animationTime / wobbleDuration));

            mat.SetFloat(Shader.PropertyToID("_DeformStrength"), deformStrength);

            animationTime += Time.deltaTime;

            yield return null;
        }

        wobbleAnimationRunning = false;
    }

    IEnumerator IdleTransition()
    {
        idleTransitionRunning = true;

        float animationTime = 0;
        float maxDuration = 1f;
        float currentWobbleStrength = mat.GetFloat(Shader.PropertyToID("_DeformStrength"));
        Color currentColor = mat.GetColor(Shader.PropertyToID("_CurrentColor"));

        while (animationTime < maxDuration)
        {
            float lerpT = animationTime / maxDuration;

            float deformStrength = Mathf.Lerp(currentWobbleStrength, idleDeformStrength, lerpT);
            mat.SetFloat(Shader.PropertyToID("_DeformStrength"), deformStrength);

            mat.SetColor(Shader.PropertyToID("_CurrentColor"), Color.Lerp(currentColor, idleColor, lerpT));

            animationTime += Time.deltaTime;

            yield return null;
        }

        idleTransitionRunning = false;
    }

    IEnumerator Dissolve()
    {
        winDissolveStart.Play();

        yield return new WaitForSeconds(1f);
        
        AudioManager.instance.Play("Win Leaves");

        yield return new WaitForSeconds(1.5f);
        
        foreach (var pSystem in winVFX)
        {
            pSystem.Play();
        }

        sparkleVFX.Stop();
        sparkleLeavesVFX.Stop();

        float timer = 0;
        float duration = 6f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            mat.SetFloat(Shader.PropertyToID("_DissolveFillAmount"), Mathf.Clamp(1 - timer / duration, 0, 1));

            yield return null;
        }
    }

    void WinFeedback()
    {
        StartCoroutine(Dissolve());
        
        AudioManager.instance.Stop("Win Static");
        AudioManager.instance.Play("Win Jingle");
        AudioManager.instance.Play("Win Wind");
        
        winWaveVFX.Play();
    }

    #endregion
}
