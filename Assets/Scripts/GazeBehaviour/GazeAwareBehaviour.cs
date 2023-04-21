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
    
    void Start()
    {
        myGaze = GetComponent<GazeAware>();
    }
    
    void Update()
    {
        if (myGaze.HasGazeFocus && !hasFocus) // Object bekommt Fokus
        {
            hasFocus = true;
            OnFocusStart();
        }
        else if (myGaze.HasGazeFocus)
        {
            OnFocusIsActive();
        }
        else if (!myGaze.HasGazeFocus && hasFocus) // Objekt verliert Fokus
        {
            hasFocus = false;
            OnFocusEnd();
        }
    }

    void OnMouseEnter()
    {
        if (onMouseDebug)
        {
            OnFocusStart();
            isMouseOver = true;
        }
    }
    void OnMouseExit()
    {
        if (onMouseDebug)
        {
            OnFocusEnd();
            isMouseOver = false;
        }
    }
    void OnMouseOver()
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
