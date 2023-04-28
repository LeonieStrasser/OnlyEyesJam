using NaughtyAttributes;
using Tobii.Gaming;
using UnityEngine;

public class GazeManager : MonoBehaviour
{
    public static GazeManager Instance;
    
    [HideInInspector] public Vector3 gazePosition;
    
    [Header("References")]
    [SerializeField] RectTransform gazeIndicator;

    [Header("General Settings")]
    [SerializeField] bool useMouseAsGaze;
    [SerializeField] bool enableGazeIndicator;
    [SerializeField] bool enableGazeSmoothing = true;
    [SerializeField] bool debug;
    
    [Header("Telekinesis Settings")]
    [SerializeField] public float telekinesisMaxDuration = 5;
    [SerializeField] public float timeTillTelekinesis = 3;
    [SerializeField] float blinkThreshold = 0.6f;

    [SerializeField] bool limitTelekinesisRadius = false;
    [ShowIf("limitTelekinesisRadius")]
    [SerializeField] float impactDistanceMax = 800;
    [SerializeField] float objectPutDownDistance = 1.5f;

    [MinMaxSlider(0f, 50f)] [SerializeField]
    Vector2 followSpeed;
    [SerializeField] float forcePower;
    [MinMaxSlider(0f, 5f)] [SerializeField]
    Vector2 minMaxDrag;
    [SerializeField] float throwVelocity = 5.5f;

    GameObject currentLookingAt, currentAttachedObject;
    ObjectState currentFocusedObjectState;

    Camera mainCam;

    Rigidbody attachedRb;

    Vector2 objectPosOnScreen;
    Vector2 directionToTarget;

    float currentBlinkDuration;
    float currentGazeDuration;
    float currentTelekinesisDuration;
    float currentImpactDistance;
    float distanceToGaze;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TobiiAPI.Start(new TobiiSettings());

        useMouseAsGaze = !TobiiAPI.IsConnected;
        
        gazeIndicator.gameObject.SetActive(enableGazeIndicator);

        mainCam = Camera.main;

        currentGazeDuration = 0;
    }

    void Update()
    {
        UpdateGazePosition();

        DetectObjectSwitch();

        BlinkDetection();
        
        if (currentAttachedObject) // ugly, aber falls das Objekt irgendwo detached wird muss das leider sein
            UpdateTelekinesisUI();
        
        if (currentAttachedObject)
            ObjectMovement();
        
        if (currentAttachedObject)
            CheckForObjectPutDown();
    }

    void UpdateGazePosition()
    {
        Vector3 rawGazePos = useMouseAsGaze ? Input.mousePosition : TobiiAPI.GetGazePoint().Screen;
        
        if (enableGazeSmoothing)
            gazePosition = Vector3.Lerp(gazePosition, rawGazePos, Time.deltaTime * 35);

        else
            gazePosition = rawGazePos;

        gazeIndicator.position = gazePosition;
    }

    void UpdateTelekinesisUI()
    {
        currentTelekinesisDuration += Time.deltaTime;

        if (!limitTelekinesisRadius)
            return;
        
        currentImpactDistance = Mathf.Lerp(impactDistanceMax, 0, currentTelekinesisDuration / telekinesisMaxDuration);
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

                if(currentFocusedObjectState)
                    currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.Neutral);
                
                currentFocusedObjectState = currentLookingAt.GetComponent<ObjectState>();
                currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.LookedAt);
            }

            if (currentFocusedObjectState?.physicalState != ObjectState.physicalStates.Immovable)
            {
                currentGazeDuration += Time.deltaTime;

                // telekinesis channeling is halfway done
                if (currentGazeDuration / timeTillTelekinesis >= 0.5f)
                {
                    GazeIndicator.Instance.StartFocusAnim();
                    currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.CloseToAttach);
                }

                // Attach new Object
                if (currentGazeDuration >= timeTillTelekinesis || currentFocusedObjectState?.physicalState ==
                    ObjectState.physicalStates.Catchable)
                {
                    Attach(currentLookingAt);
                    currentGazeDuration = 0;
                }
            }
        }

        else
        {
            currentLookingAt = null;
            currentGazeDuration = 0;

            if(currentFocusedObjectState)
                currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.Neutral);
            
            currentFocusedObjectState = null;
            
            GazeIndicator.Instance.EndFocusAnim();
        }
    }

    GameObject GetFocusedObject()
    {
        if (useMouseAsGaze)
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
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
        if (currentTelekinesisDuration >= telekinesisMaxDuration)
        {
            currentFocusedObjectState?.ChangePhysicalState(ObjectState.physicalStates.Falling);
            Detach();
            return;
        }
        
        CalculateGazeData();

        //--------------DEBUGGING
        if (debug)
        {
            Debug.DrawRay(currentAttachedObject.transform.position, directionToTarget * 2, Color.cyan);
            Debug.Log("Distance to Gaze is " + distanceToGaze);
        }

        if ((distanceToGaze < currentImpactDistance || !limitTelekinesisRadius) && attachedRb.velocity.magnitude < throwVelocity)
            MoveInGazeDirection();

        else
        {
            //currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Catchable);
            currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Falling);
            Detach();
        }
    }

    void CheckForObjectPutDown()
    {
        Debug.DrawLine(objectPosOnScreen, new Vector3(objectPosOnScreen.x, objectPosOnScreen.y - objectPutDownDistance));
        
        if (currentFocusedObjectState.hasSomethingUnderneath &&
            gazePosition.y < objectPosOnScreen.y - objectPutDownDistance)
        {
            Detach();

            if (debug)
                Debug.Log("Object was put down");
        }
    }

    void Attach(GameObject _objToAttach)
    {
        if (debug)
            Debug.Log("Attach!");

        currentAttachedObject = _objToAttach;
        attachedRb = currentAttachedObject.GetComponent<Rigidbody>();
        attachedRb.useGravity = false;
        attachedRb.velocity *= 0.5f;
        attachedRb.drag = minMaxDrag.y;

        currentAttachedObject.tag = "Attached";
        
        currentFocusedObjectState.ChangeVisualState(ObjectState.visualStates.Attached);
        currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Attached);

        currentTelekinesisDuration = 0;
        currentImpactDistance = impactDistanceMax;
    }

    void Detach()
    {
        if (debug)
            Debug.Log("Detach!");
        
        currentAttachedObject.tag = "MoveableObject";

        attachedRb.useGravity = true;
        attachedRb.drag = minMaxDrag.x;
        attachedRb = null;
        currentAttachedObject = null;

        currentFocusedObjectState.ChangeVisualState(ObjectState.visualStates.Neutral);
        currentFocusedObjectState = null;
    }

    void CalculateGazeData()
    {
        Vector3 targetPos = useMouseAsGaze ? Input.mousePosition : TobiiAPI.GetGazePoint().Screen;
        
        // Gaze Direction
        objectPosOnScreen = mainCam.WorldToScreenPoint(currentAttachedObject.transform.position);
        directionToTarget = new Vector2(targetPos.x - objectPosOnScreen.x, targetPos.y - objectPosOnScreen.y)
            .normalized;

        // Gaze Distance
        distanceToGaze = Vector2.Distance(objectPosOnScreen, targetPos);
    }

    void MoveInGazeDirection()
    {
        float currentFollowSpeed = Mathf.Lerp(followSpeed.x, followSpeed.y, distanceToGaze / impactDistanceMax);

        if (distanceToGaze > 15f)
            attachedRb.AddForce((Vector3)directionToTarget * (currentFollowSpeed * Time.deltaTime * forcePower));
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

                currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Falling);
                Detach();
            }
        }

        else
            currentBlinkDuration = 0;
    }
}


