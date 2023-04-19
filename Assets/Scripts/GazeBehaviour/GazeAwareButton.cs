using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GazeAwareButton : GazeAwareBehaviour
{
    [SerializeField] float klickTime;
    [SerializeField] UnityEvent OnHoverEvents;
    [SerializeField] UnityEvent OnDehoverEvents;
    [SerializeField] UnityEvent OnKlickEvents;


    // Klicktimer
    float klickTimer;

    private void Start()
    {
        ResetKlickTimer();
    }

    private void Update()
    {

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
            // Timer läuft jeden Frame ab

            klickTimer -= Time.deltaTime;
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
