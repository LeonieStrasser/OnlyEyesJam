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
    [SerializeField] bool useShader;

    private void Awake()
    {
        if(useShader)
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
        Debug.Log(IsObjectGazed());
        if (IsObjectGazed())
        {
            // Timer laeuft jeden Frame ab

            klickTimer -= Time.deltaTime;

            if(useShader)
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
        
        if(useShader)
            buttonMat.SetFloat("_FillAmount", 0);
    }

    void ActivateKlick()
    {
        OnKlickEvents.Invoke();

        OnKlick();
    }

    protected virtual void OnKlick()
    {

    }
    protected virtual void OnHover()
    {

    }
    protected virtual void OnHoverEnd()
    {

    }
}
