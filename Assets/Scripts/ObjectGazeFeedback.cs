using System;
using UnityEngine;

public class ObjectGazeFeedback : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    Color standardColor;
    int currentFeedbackIndex;

    void Start()
    {
        standardColor = meshRenderer.material.color;
    }

    public void PlayFeedback(int _feedbackIndex)
    {
        Debug.Log("Play Feedback " + _feedbackIndex);
        
        if(_feedbackIndex == currentFeedbackIndex)
            return;

        currentFeedbackIndex = _feedbackIndex;
        
        switch (_feedbackIndex)
        {
            case 1:
                meshRenderer.material.color = Color.red;
                break;
            case 2:
                meshRenderer.material.color = Color.yellow;
                break;
            case 3:
                meshRenderer.material.color = Color.green;
                break;
        }
    }

    public void StopFeedback()
    {
        currentFeedbackIndex = 0;
        meshRenderer.material.color = standardColor;
    }
}
