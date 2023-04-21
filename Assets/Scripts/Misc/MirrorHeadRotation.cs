using UnityEngine;
using Tobii.Gaming;

public class MirrorHeadRotation : MonoBehaviour
{
    void Start()
    {
        TobiiAPI.Start(new TobiiSettings());
    }

    void Update()
    {
        //Debug.Log(TobiiAPI.GetHeadPose().Rotation);
        //Debug.Log(TobiiAPI.GetHeadPose().Position);
        ApplyRotation();
    }

    void ApplyRotation()
    {
        transform.rotation = TobiiAPI.GetHeadPose().Rotation;
    }
}