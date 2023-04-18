using Tobii.Gaming;
using UnityEngine;

public class GazeManager : MonoBehaviour
{
    [SerializeField] RectTransform gazeIndicator;
    [SerializeField] float blinkThreshold;

    [SerializeField] float timeTillTelekinesis = 5;
    [SerializeField] float releaseDelayTime = 2;
    [SerializeField] float gazeFollowSpeed = 0.2f;
    [SerializeField] float impactDistance = 5;
    
    [SerializeField] bool debug;

    GameObject currentLookingAt, currentAttachedObject;
    
    Rigidbody attachedRb;
    
    float currentBlinkDuration;
    float currentGazeDuration;
    float distanceToGaze;
    
    Vector2 objectPosOnScreen;
    Vector2 directionToTarget;
    
    void Start()
    {
        TobiiAPI.Start(new TobiiSettings());
        
        currentGazeDuration = 0;
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
                currentGazeDuration = timeTillTelekinesis;
            }

            currentGazeDuration -= Time.deltaTime;
            
            // Attach new Object
            if (currentGazeDuration <= 0)
            {
                Attach(currentLookingAt);
                currentGazeDuration = timeTillTelekinesis;
            }
        }
        
        else
        {
            currentLookingAt = null;
            currentGazeDuration = timeTillTelekinesis;
        }
    }

    void ObjectMovement()
    {
        if(!currentAttachedObject)
            return;
        
        CalculateGazeData();

        if (distanceToGaze < impactDistance)
        {
            MoveInGazeDirection();
        }

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
        attachedRb = currentAttachedObject.GetComponent<Rigidbody>();
        attachedRb.useGravity = false;
    }
    
    void Detach()
    {
        if(debug)
            Debug.Log("Detach!");
        
        attachedRb.useGravity = true;
        attachedRb = null;
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
        attachedRb.transform.position += ((Vector3)directionToTarget * gazeFollowSpeed * Time.deltaTime);
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
}

