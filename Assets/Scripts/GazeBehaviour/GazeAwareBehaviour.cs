using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

[RequireComponent(typeof(GazeAware))]
public class GazeAwareBehaviour : MonoBehaviour
{
    [SerializeField] bool onMouseDebug;

    GazeAware myGaze;
    bool hasFocus;

    bool isMouseOver;

    // Start is called before the first frame update
    void Start()
    {
        myGaze = GetComponent<GazeAware>();

        // TobiiAPI.Start(new TobiiSettings()); // muss das nur einmal in der szene gecallt werden oder per script?

    }

    // Update is called once per frame
    void Update()
    {

        if (myGaze.HasGazeFocus && !hasFocus) // Object bekommt Focus
        {
            hasFocus = true;
            OnFocusStart();
            return;
        }
        else if (myGaze.HasGazeFocus)
        {
            OnFocusIsActive();
            return;
        }
        else if (!myGaze.HasGazeFocus && hasFocus) // Objekt verliert Fokus
        {
            hasFocus = false;
            OnFocusEnd();
            return;
        }

    }

    private void OnMouseEnter()
    {
        if (onMouseDebug)
        {
            OnFocusStart();
            isMouseOver = true;
        }
    }
    private void OnMouseExit()
    {
        if (onMouseDebug)
        {
            OnFocusEnd();
            isMouseOver = false;
        }
    }
    private void OnMouseOver()
    {
        if (onMouseDebug)
        {
            OnFocusIsActive();
        }
    }


    protected virtual void OnFocusStart()
    {
        Debug.Log("Focus started", gameObject);
    }

    protected virtual void OnFocusEnd()
    {
        Debug.Log("Focus ended", gameObject);
    }

    protected virtual void OnFocusIsActive()
    {

    }


    public virtual bool IsObjectGazed()
    {
        if (!onMouseDebug)
            return myGaze.HasGazeFocus;
        else
            return isMouseOver;
    }
}
