using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeAwareButton : GazeAwareBehaviour
{
    [SerializeField] float klickTime;
    
    [SerializeField] MeshRenderer buttonMesh;
    Material buttonMat;
    
    [SerializeField] UnityEvent OnHoverEvents;
    [SerializeField] UnityEvent OnDehoverEvents;
    [SerializeField] UnityEvent OnKlickEvents;

    // Klicktimer
    float klickTimer;

    private void Awake()
    {
        if(buttonMesh)
            buttonMat = buttonMesh.material;
        
        ResetKlickTimer();
    }

    /*void OnEnable()
    {
        buttonMat = buttonMesh.material;
        Debug.Log("GOT MAT? :" + buttonMat);
    }*/

    protected override void OnFocusStart()
    {
        base.OnFocusStart();

        OnHoverEvents.Invoke();

        OnHover();
    }

    protected override void OnFocusEnd()
    {
        base.OnFocusEnd();

        OnDehoverEvents.Invoke();
        OnHoverEnd();

        ResetKlickTimer();
    }

    protected override void OnFocusIsActive()
    {
        base.OnFocusIsActive();

        KlicktimerProgress();
    }

    void KlicktimerProgress()
    {
        if (IsObjectGazed())
        {
            // Timer laeuft jeden Frame ab

            klickTimer -= Time.deltaTime;
            
            if(buttonMesh)
                buttonMat.SetFloat("_FillAmount", 1 - klickTimer / klickTime);
            
            if (klickTimer <= 0)
            {
                ActivateKlick();
                ResetKlickTimer();
            }
        }
    }

    void ResetKlickTimer()
    {
        klickTimer = klickTime;
        
        if(buttonMesh)
            buttonMat.SetFloat("_FillAmount", 0);
    }

    void ActivateKlick()
    {
        OnKlick();
        OnKlickEvents.Invoke();

        
    }

    protected virtual void OnKlick()
    {
        AudioManager.instance.Play("UI Select");
    }
    protected virtual void OnHover()
    {
        AudioManager.instance.Play("UI Static");
    }
    protected virtual void OnHoverEnd()
    {
        AudioManager.instance.Stop("UI Static");
    }
}
