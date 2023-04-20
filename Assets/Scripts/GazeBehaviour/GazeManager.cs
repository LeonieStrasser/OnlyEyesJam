using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Tobii.Gaming;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GazeManager : MonoBehaviour
{
    public static GazeManager Instance;
    
    public Vector3 gazePosition;
    
    [SerializeField] RectTransform gazeIndicator;
    [SerializeField] RectTransform radiusIndicator;
    [SerializeField] float blinkThreshold;

    [SerializeField] float timeTillTelekinesis = 3;
    [SerializeField] float telekinesisMaxDuration = 5;

    [MinMaxSlider(0f, 50f)] [SerializeField]
    Vector2 followSpeed;

    [SerializeField] float impactDistanceMax = 800;

    [SerializeField] bool debug;
    [SerializeField] bool useMouseAsGaze;

    GameObject currentLookingAt, currentAttachedObject;
    ObjectState currentFeedback;

    Rigidbody attachedRb;

    Image indicatorFill;

    float currentBlinkDuration;
    float currentGazeDuration;
    float currentTelekinesisDuration;
    float currentImpactDistance;
    float distanceToGaze;

    Vector2 objectPosOnScreen;
    Vector2 directionToTarget;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TobiiAPI.Start(new TobiiSettings());

        indicatorFill = gazeIndicator.GetChild(0).GetComponent<Image>();

        currentGazeDuration = 0;
    }

    void Update()
    {
        gazePosition = useMouseAsGaze ? Input.mousePosition : TobiiAPI.GetGazePoint().Screen;

        gazeIndicator.position = gazePosition;

        DetectObjectSwitch();

        BlinkDetection();
        
        if (currentAttachedObject)
        {
            UpdateTelekinesisUI();
            
            ObjectMovement();
        }
    }

    void UpdateTelekinesisUI()
    {
        currentTelekinesisDuration += Time.deltaTime;

        currentImpactDistance = Mathf.Lerp(impactDistanceMax, 0, currentTelekinesisDuration / telekinesisMaxDuration);
        
        RectTransform rt = radiusIndicator;
        rt.position = objectPosOnScreen;
        rt.sizeDelta = new Vector2(currentImpactDistance * 2, currentImpactDistance * 2);

        if (currentTelekinesisDuration >= telekinesisMaxDuration)
        {
            Detach();
        }
    }

    void DetectObjectSwitch()
    {
        if (currentAttachedObject)
            return;

        GameObject focusedObject = GetFocusedObject();

        if (focusedObject && focusedObject.CompareTag("MoveableObject"))
        {
            // Looking at new object
            if (currentLookingAt != focusedObject)
            {
                currentLookingAt = focusedObject;
                currentGazeDuration = 0;
                indicatorFill.fillAmount = 0;
                
                if(currentFeedback)
                    currentFeedback.ChangeVisualState(ObjectState.visualStates.Neutral);
                
                currentFeedback = currentLookingAt.GetComponent<ObjectState>();
                currentFeedback.ChangeVisualState(ObjectState.visualStates.LookedAt);
            }

            currentGazeDuration += Time.deltaTime;
            indicatorFill.fillAmount = currentGazeDuration / timeTillTelekinesis;
            
            if(currentGazeDuration >= 1f)
                currentFeedback.ChangeVisualState(ObjectState.visualStates.CloseToAttach);

            // Attach new Object
            if (currentGazeDuration >= timeTillTelekinesis || currentFeedback.physicalState == ObjectState.physicalStates.Falling)
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
            
            if(currentFeedback)
                currentFeedback.ChangeVisualState(ObjectState.visualStates.Neutral);
            
            currentFeedback = null;
        }
    }

    GameObject GetFocusedObject()
    {
        if (useMouseAsGaze)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponent<GazeAware>())
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        return TobiiAPI.GetFocusedObject();
    }

    void ObjectMovement()
    {
        CalculateGazeData();

        //--------------DEBUGGING
        if (debug)
        {
            Debug.DrawRay(currentAttachedObject.transform.position, directionToTarget * 2, Color.cyan);
            Debug.Log("Distance to Gaze is " + distanceToGaze);
        }

        if (distanceToGaze < currentImpactDistance)
            MoveInGazeDirection();

        else
            Detach();
    }

    void Attach(GameObject _objToAttach)
    {
        if (debug)
            Debug.Log("Attach!");

        currentAttachedObject = _objToAttach;
        attachedRb = currentAttachedObject.GetComponent<Rigidbody>();
        attachedRb.useGravity = false;
        attachedRb.velocity *= 0.5f;

        currentAttachedObject.tag = "Attached";
        
        currentFeedback.ChangeVisualState(ObjectState.visualStates.Attached);
        currentFeedback.ChangePhysicalState(ObjectState.physicalStates.Attached);

        currentTelekinesisDuration = 0;
        currentImpactDistance = impactDistanceMax;

        radiusIndicator.gameObject.SetActive(true);
    }

    void Detach()
    {
        if (debug)
            Debug.Log("Detach!");
        
        currentAttachedObject.tag = "MoveableObject";

        attachedRb.useGravity = true;
        attachedRb = null;
        currentAttachedObject = null;

        currentFeedback.ChangeVisualState(ObjectState.visualStates.Neutral);
        currentFeedback.ChangePhysicalState(ObjectState.physicalStates.Falling);
        currentFeedback = null;

        radiusIndicator.gameObject.SetActive(false);
    }

    void CalculateGazeData()
    {
        Vector3 targetPos = useMouseAsGaze ? Input.mousePosition : TobiiAPI.GetGazePoint().Screen;

        // Gaze Direction
        objectPosOnScreen = Camera.main.WorldToScreenPoint(currentAttachedObject.transform.position);
        directionToTarget = new Vector2(targetPos.x - objectPosOnScreen.x, targetPos.y - objectPosOnScreen.y)
            .normalized;

        // Gaze Distance
        distanceToGaze = Vector2.Distance(objectPosOnScreen, targetPos);
    }

    void MoveInGazeDirection()
    {
        float currentFollowSpeed = Mathf.Lerp(followSpeed.x, followSpeed.y, distanceToGaze / impactDistanceMax);
        
        if(distanceToGaze > 15f)
            attachedRb.transform.position += ((Vector3)directionToTarget * currentFollowSpeed * Time.deltaTime);
    }

    void BlinkDetection()
    {
        if (!currentAttachedObject)
            return;

        bool gazeFound = useMouseAsGaze ? !Input.GetMouseButton(0) : TobiiAPI.GetGazePoint().IsRecent();

        if (!gazeFound)
        {
            currentBlinkDuration += Time.deltaTime;

            if (currentBlinkDuration >= blinkThreshold)
            {
                if (debug)
                    Debug.Log("Drop Object!");

                currentBlinkDuration = 0;

                Detach();
            }
        }

        else
            currentBlinkDuration = 0;
    }
}


