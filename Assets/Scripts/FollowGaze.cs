using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowGaze : MonoBehaviour
{
    [SerializeField] float gazeFollowSpeed = 0.2f;
    [SerializeField] float impactDistance = 5;

    public bool debug = true;
    Vector2 objectPosOnScreen;
    Vector2 directionToMousePos;
    float distanceToGaze;

    Rigidbody rb;

    // DEBUG
    public Image testImagge;


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
            testImagge.rectTransform.position = objectPosOnScreen;
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
