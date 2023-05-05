using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Camera cam;
    
    [SerializeField] float rotationStrength = 0.01f;
    [SerializeField] float rotationSpeed = 1f;

    void Start()
    {
        cam = GetComponent<Camera>();
        
        Debug.Log(cam);
    }

    void Update()
    {
        Vector3 gazePos = GazeManager.Instance.gazePosition;

        gazePos = gazePos.magnitude < 5000f ? gazePos : Vector3.zero;

        Vector3 offsetFromCenter = gazePos - new Vector3(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f, 0);

        Quaternion newRot = Quaternion.Euler(new Vector3(-offsetFromCenter.y, offsetFromCenter.x, 0) * (rotationStrength * 0.01f));

        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * rotationSpeed);
    }
}
