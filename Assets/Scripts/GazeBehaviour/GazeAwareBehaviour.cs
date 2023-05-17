using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.Android;

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
        GazeManager.Instance.SwitchMouseDebug += ChangeMouseDebug;

        onMouseDebug = !TobiiAPI.IsConnected;
    }

    void ChangeMouseDebug(bool _useMouse)
    {
        onMouseDebug = _useMouse;
        Debug.Log("Event fired: " + _useMouse);
    }
    
    void Update()
    {
        if(onMouseDebug)
            return;
        
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

    void OnDestroy()
    {
        GazeManager.Instance.SwitchMouseDebug -= ChangeMouseDebug;
    }

    protected virtual void OnFocusStart()
    {
        
    }

    protected virtual void OnFocusEnd()
    {
        
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
