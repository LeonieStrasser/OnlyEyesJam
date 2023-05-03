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

    [SerializeField] bool makeSound;

    bool interactible = true;

    // Klicktimer
    float klickTimer;

    private void Awake()
    {
        if(buttonMesh)
            buttonMat = buttonMesh.material;

        interactible = true;
        ResetKlickTimer();
    }

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


            if (interactible)
            {
                klickTimer -= Time.deltaTime;


                if (buttonMesh)
                    buttonMat.SetFloat("_FillAmount", 1 - klickTimer / klickTime);
            }
                
                


            
            if (klickTimer <= 0)
            {
                StartCoroutine(ActivateKlick());
                ResetKlickTimer();
            }
        }
    }

    void ResetKlickTimer()
    {
        klickTimer = klickTime;
        
        if(interactible)
            if(buttonMesh)
                buttonMat.SetFloat("_FillAmount", 0);
    }

    private IEnumerator ActivateKlick()
    {
        OnKlick();
        interactible = false;
        yield return new WaitForSeconds(AudioManager.instance.GetLength("UI Select"));
        OnKlickEvents.Invoke();

        
    }

    protected virtual void OnKlick()
    {
        if(makeSound)
            AudioManager.instance.Play("UI Select");
    }
    protected virtual void OnHover()
    {
        if(makeSound)
            AudioManager.instance.Play("UI Static");
    }
    protected virtual void OnHoverEnd()
    {
        if(makeSound)
            AudioManager.instance.Stop("UI Static");
    }

}
