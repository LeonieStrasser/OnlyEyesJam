using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GazeManager : MonoBehaviour
{
    [SerializeField] RectTransform gazeIndicator;
    [SerializeField] float blinkThreshold;
    float currentBlinkDuration;

    [SerializeField] float timeTillTelekinese = 5;
    [SerializeField] float releaseDelayTime = 2;
    [SerializeField] float telekineseForce = 0.2f;
    [SerializeField] bool debug;

    public GameObject currentLookingAt, currentAttachedObject;

    Animator anim;
    GazeAware gazeAware;
    float gazeTargetTime;
    float gazeChannelTime;
    bool isInTelekinese;
    
    [SerializeField] float gazeFollowSpeed = 0.2f;
    [SerializeField] float impactDistance = 5;
    
    Vector2 objectPosOnScreen;
    Vector2 directionToTarget;
    float distanceToGaze;

    Rigidbody rb;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        gazeAware = GetComponent<GazeAware>();
        isInTelekinese = false;

        gazeTargetTime = 0;
        gazeChannelTime = 0;

        TobiiAPI.Start(new TobiiSettings());
    }

    void Update()
    {
        gazeIndicator.position = TobiiAPI.GetGazePoint().Screen;
        
        DetectObjectSwitch();
        
        BlinkDetection();

        ObjectMovement();
    }

    void DetectObjectSwitch()
    {
        GameObject focusedObject = TobiiAPI.GetFocusedObject();

        if (focusedObject && !currentAttachedObject)
        {
            // Looking at new object
            if (currentLookingAt != focusedObject)
            {
                currentLookingAt = focusedObject;
                gazeChannelTime = timeTillTelekinese;
            }

            gazeChannelTime -= Time.deltaTime;
            
            // Attach new Object
            if (gazeChannelTime <= 0)
            {
                Attach(currentLookingAt);
                gazeChannelTime = timeTillTelekinese;
            }
        }
        
        else
        {
            currentLookingAt = null;
            gazeChannelTime = timeTillTelekinese;
        }
    }

    void ObjectMovement()
    {
        if(!currentAttachedObject)
            return;
        
        CalculateGazeData();

        /*if (distanceToGaze < impactDistance)
        {
            MoveInGazeDirection();
        }*/
        
        MoveInGazeDirection();
        
        //--------------DEBUGGING

        if (debug)
        {
            Debug.DrawRay(transform.position, directionToTarget * 2, Color.cyan);
            Debug.Log("Distance to Gaze is " + distanceToGaze);
        }
    }

    void Attach(GameObject _objToAttach)
    {
        if(debug)
            Debug.Log("Attach!");
        
        currentAttachedObject = _objToAttach;
        rb = currentAttachedObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    
    void Detach()
    {
        if(debug)
            Debug.Log("Detach!");
        
        rb.useGravity = true;
        rb = null;
        currentAttachedObject = null;
    }
    
    void CalculateGazeData()
    {
        Vector3 targetPos = TobiiAPI.GetGazePoint().Screen;
        
        // Gaze Direction
        objectPosOnScreen = Camera.main.WorldToScreenPoint(currentAttachedObject.transform.position);
        directionToTarget = new Vector2(targetPos.x - objectPosOnScreen.x, targetPos.y - objectPosOnScreen.y).normalized;

        // Gaze Distance
        distanceToGaze = Vector2.Distance(objectPosOnScreen, targetPos);
    }


    void MoveInGazeDirection()
    {
        rb.transform.position += ((Vector3)directionToTarget * gazeFollowSpeed * Time.deltaTime);
    }

    void BlinkDetection()
    {
        if(!currentAttachedObject)
            return;
        
        bool gazeFound = TobiiAPI.GetGazePoint().IsRecent();

        if (!gazeFound)
        {
            currentBlinkDuration += Time.deltaTime;

            if (currentBlinkDuration >= blinkThreshold)
            {
                if(debug)
                    Debug.Log("Drop Object!");
                
                currentBlinkDuration = 0;
                
                Detach();
            }
        }

        else
            currentBlinkDuration = 0;
    }

    void WhileLookedAt()
    {
        gazeTargetTime += Time.deltaTime;

        if (gazeTargetTime >= timeTillTelekinese)
        {
            rb.AddForce(new Vector3(0, 1 * telekineseForce, 0), ForceMode.Impulse);
            gazeChannelTime = releaseDelayTime;
        }
    }

    void LookStart()
    {
        anim.SetBool("lookedAt", true);
        isInTelekinese = true;
        rb.useGravity = false;
    }

    void LookEnd()
    {
        anim.SetBool("lookedAt", false);
        isInTelekinese = false;
        rb.useGravity = true;
        gazeTargetTime = 0;
    }
}

