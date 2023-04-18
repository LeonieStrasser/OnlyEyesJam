using NaughtyAttributes;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GazeManager : MonoBehaviour
{
    [SerializeField] RectTransform gazeIndicator;
    [SerializeField] RectTransform radiusIndicator;
    [SerializeField] float blinkThreshold;

    [SerializeField] float timeTillTelekinesis = 5;
    [SerializeField] float releaseDelayTime = 2;

    [MinMaxSlider(0f, 50f)]
    [SerializeField] Vector2 followSpeed;
    [SerializeField] float impactDistance = 5;
    
    [SerializeField] bool debug;

    GameObject currentLookingAt, currentAttachedObject;
    
    Rigidbody attachedRb;

    Image indicatorFill;
    
    float currentBlinkDuration;
    float currentGazeDuration;
    float distanceToGaze;
    
    Vector2 objectPosOnScreen;
    Vector2 directionToTarget;
    
    void Start()
    {
        TobiiAPI.Start(new TobiiSettings());

        indicatorFill = gazeIndicator.GetChild(0).GetComponent<Image>();
        
        currentGazeDuration = 0;
    }

    void Update()
    {
        gazeIndicator.position = TobiiAPI.GetGazePoint().Screen;

        DetectObjectSwitch();
        
        BlinkDetection();

        ObjectMovement();
        
        if (currentAttachedObject)
        {
            RectTransform rt = radiusIndicator;
            rt.position = objectPosOnScreen;
            rt.sizeDelta = new Vector2(impactDistance * 2, impactDistance * 2);
        }
    }

    void DetectObjectSwitch()
    {
        if(currentAttachedObject)
            return;
        
        GameObject focusedObject = TobiiAPI.GetFocusedObject();

        if (focusedObject)
        {
            // Looking at new object
            if (currentLookingAt != focusedObject)
            {
                currentLookingAt = focusedObject;
                currentGazeDuration = 0;
                indicatorFill.fillAmount = 0;
            }

            currentGazeDuration += Time.deltaTime;
            indicatorFill.fillAmount = currentGazeDuration / timeTillTelekinesis;
            
            // Attach new Object
            if (currentGazeDuration >= timeTillTelekinesis)
            {
                Attach(currentLookingAt);
                currentGazeDuration = 0;
            }
        }
        
        else
        {
            currentLookingAt = null;
            currentGazeDuration = 0;
            indicatorFill.fillAmount = 0;
        }
    }

    void ObjectMovement()
    {
        if(!currentAttachedObject)
            return;
        
        CalculateGazeData();
        
        //--------------DEBUGGING
        if (debug)
        {
            Debug.DrawRay(currentAttachedObject.transform.position, directionToTarget * 2, Color.cyan);
            Debug.Log("Distance to Gaze is " + distanceToGaze);
        }

        if (distanceToGaze < impactDistance)
            MoveInGazeDirection();

        else
            Detach();
    }

    void Attach(GameObject _objToAttach)
    {
        if(debug)
            Debug.Log("Attach!");
        
        currentAttachedObject = _objToAttach;
        attachedRb = currentAttachedObject.GetComponent<Rigidbody>();
        attachedRb.useGravity = false;
        
        radiusIndicator.gameObject.SetActive(true);
    }
    
    void Detach()
    {
        if(debug)
            Debug.Log("Detach!");
        
        attachedRb.useGravity = true;
        attachedRb = null;
        currentAttachedObject = null;
        
        radiusIndicator.gameObject.SetActive(false);
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
        float currentFollowSpeed = Mathf.Lerp(followSpeed.x, followSpeed.y, distanceToGaze / impactDistance);
        
        attachedRb.transform.position += ((Vector3)directionToTarget * currentFollowSpeed * Time.deltaTime);
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

