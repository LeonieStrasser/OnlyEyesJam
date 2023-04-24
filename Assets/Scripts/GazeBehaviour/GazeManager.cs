using NaughtyAttributes;
using Tobii.Gaming;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class GazeManager : MonoBehaviour
{
    public static GazeManager Instance;
    
    [HideInInspector] public Vector3 gazePosition;
    
    [Header("References")]
    [SerializeField] RectTransform gazeIndicator;
    [SerializeField] RectTransform radiusIndicator;
    
    [Header("General Settings")]
    [SerializeField] bool useMouseAsGaze;
    [SerializeField] bool enableGazeSmoothing = true;
    [SerializeField] bool debug;
    
    [Header("Telekinesis Settings")]
    [SerializeField] float telekinesisMaxDuration = 5;
    [SerializeField] float timeTillTelekinesis = 3;
    [SerializeField] float blinkThreshold = 0.6f;

    [SerializeField] bool limitTelekinesisRadius = false;
    [ShowIf("limitTelekinesisRadius")]
    [SerializeField] float impactDistanceMax = 800;

    [MinMaxSlider(0f, 50f)] [SerializeField]
    Vector2 followSpeed;
    [SerializeField] float throwVelocity = 5.5f;

    GameObject currentLookingAt, currentAttachedObject;
    ObjectState currentFocusedObjectState;

    Rigidbody attachedRb;

    Image indicatorFill;
    
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

        indicatorFill = gazeIndicator.GetChild(0).GetComponent<Image>();

        currentGazeDuration = 0;
    }

    void Update()
    {
        UpdateGazePosition();

        DetectObjectSwitch();

        BlinkDetection();
        
        if (currentAttachedObject)
        {
            UpdateTelekinesisUI();
            
            ObjectMovement();
        }
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

        if (currentTelekinesisDuration >= telekinesisMaxDuration)
        {
            currentFocusedObjectState?.ChangePhysicalState(ObjectState.physicalStates.Falling);
            Detach();
        }

        if (!limitTelekinesisRadius)
            return;
        
        currentImpactDistance = Mathf.Lerp(impactDistanceMax, 0, currentTelekinesisDuration / telekinesisMaxDuration);
        
        RectTransform rt = radiusIndicator;
        rt.position = objectPosOnScreen;
        rt.sizeDelta = new Vector2(currentImpactDistance * 2, currentImpactDistance * 2);
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
                
                if(currentFocusedObjectState)
                    currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.Neutral);
                
                currentFocusedObjectState = currentLookingAt.GetComponent<ObjectState>();
                currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.LookedAt);
            }

            currentGazeDuration += Time.deltaTime;
            indicatorFill.fillAmount = currentGazeDuration / timeTillTelekinesis;
            
            if(currentGazeDuration / timeTillTelekinesis >= 0.5f)
                currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.CloseToAttach);

            // Attach new Object
            if (currentGazeDuration >= timeTillTelekinesis || currentFocusedObjectState?.physicalState == ObjectState.physicalStates.Catchable)
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
            
            if(currentFocusedObjectState)
                currentFocusedObjectState?.ChangeVisualState(ObjectState.visualStates.Neutral);
            
            currentFocusedObjectState = null;
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

        if ((distanceToGaze < currentImpactDistance || !limitTelekinesisRadius) && attachedRb.velocity.magnitude < throwVelocity)
            MoveInGazeDirection();

        else
        {
            //currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Catchable);
            currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Falling);
            Detach();
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

        currentAttachedObject.tag = "Attached";
        
        currentFocusedObjectState.ChangeVisualState(ObjectState.visualStates.Attached);
        currentFocusedObjectState.ChangePhysicalState(ObjectState.physicalStates.Attached);

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

        currentFocusedObjectState.ChangeVisualState(ObjectState.visualStates.Neutral);
        currentFocusedObjectState = null;

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

        if (distanceToGaze > 15f)
            //attachedRb.transform.position += ((Vector3)directionToTarget * currentFollowSpeed * Time.deltaTime);
            attachedRb.AddForce((Vector3)directionToTarget * (currentFollowSpeed * Time.deltaTime * 80f));
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


