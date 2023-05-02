using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float paraStrength;
    public Camera cam;
    public Transform subject;

    private Vector2 startPosition;
    private float startZ;
    

    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane));
    float parallaxFactor => Mathf.Abs(distanceFromSubject)/clippingPlane;

    public void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void Update()
    {
        if(cam != null && subject != null)
        {
            Vector2 newPos = transform.position = startPosition + travel * paraStrength * parallaxFactor;
            transform.position = new Vector3(newPos.x, newPos.y, startZ);
        }
        
    }


}
