using UnityEngine;
using Tobii.Gaming;

public class GazeDetection : MonoBehaviour
{
    [SerializeField] float timeTillTelekinese = 5;
    [SerializeField] float releaseDelayTime = 2;
    [SerializeField] float telekineseForce = 0.2f;

    Animator anim;
    GazeAware gazeAware;
    Rigidbody rb;
    float gazeTargetTime;
    float gazeDelayTime;
    bool isInTelekinese;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        gazeAware = GetComponent<GazeAware>();
        isInTelekinese = false;

        gazeTargetTime = 0;
        gazeDelayTime = 0;

        TobiiAPI.Start(new TobiiSettings());
    }

    void Update()
    {
        if(!isInTelekinese && gazeDelayTime > 0)
        {
            gazeDelayTime -= Time.deltaTime;
        }

        if(!isInTelekinese && gazeAware.HasGazeFocus)
            LookStart();
        
        else if(isInTelekinese && !gazeAware.HasGazeFocus)
            LookEnd();

        else if (gazeAware.HasGazeFocus)
        {
            WhileLookedAt();
        }
    }

    void WhileLookedAt()
    {
        gazeTargetTime += Time.deltaTime;

        if(gazeTargetTime >= timeTillTelekinese)
        {
            rb.AddForce(new Vector3(0, 1 * telekineseForce, 0), ForceMode.Impulse);
            gazeDelayTime = releaseDelayTime;
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
    
    public class FollowGaze : MonoBehaviour
    {
        [SerializeField] float gazeFollowSpeed = 0.2f;
        [SerializeField] float impactDistance = 5;
    
        public bool debug = true;
        Vector2 objectPosOnScreen;
        Vector2 directionToMousePos;
        float distanceToGaze;
    
        Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            CalculateGazeData();
    
            if (distanceToGaze < impactDistance)
            {
                MoveInGazeDirection();
            }
    
    
    
            //--------------DEBUGGING
    
            if (debug)
            {
                Debug.DrawRay(transform.position, directionToMousePos * 2, Color.cyan);
    
                Debug.Log("Distance to Gaze is " + distanceToGaze);
            }
        }
    
        void CalculateGazeData()
        {
            // Gaze Direction
            objectPosOnScreen = Camera.main.WorldToScreenPoint(transform.position);
            directionToMousePos = new Vector2(Input.mousePosition.x - objectPosOnScreen.x, Input.mousePosition.y - objectPosOnScreen.y).normalized;
    
            // Gaze Distance
            distanceToGaze = Vector2.Distance(objectPosOnScreen, Input.mousePosition);
        }
    
    
        void MoveInGazeDirection()
        {
            rb.transform.position = rb.transform.position + ((Vector3)directionToMousePos * gazeFollowSpeed * Time.deltaTime);
        }
    }
}
