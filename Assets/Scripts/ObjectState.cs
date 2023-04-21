using System;
using Unity.Mathematics;
using UnityEngine;

public class ObjectState : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject objectCollisionEffect;

    public enum physicalStates
    {
        Grounded, Falling, Catchable, Attached
    }

    [HideInInspector] public physicalStates physicalState;
    
    public enum visualStates
    {
        Neutral, LookedAt, CloseToAttach, Attached
    }

    [HideInInspector] public visualStates visualState;

    Color standardColor;
    int currentFeedbackIndex;

    void Start()
    {
        standardColor = meshRenderer.material.color;
        visualState = visualStates.Neutral;
        physicalState = physicalStates.Falling;
    }

    public void ChangeVisualState(visualStates _newState)
    {
        if (_newState == visualState)
            return;

        visualState = _newState;
        
        switch (_newState)
        {
            case visualStates.LookedAt:
                meshRenderer.material.color = Color.red;
                break;
            case visualStates.CloseToAttach:
                meshRenderer.material.color = Color.yellow;
                break;
            case visualStates.Attached:
                meshRenderer.material.color = Color.green;
                break;
            case visualStates.Neutral:
                currentFeedbackIndex = 0;
                meshRenderer.material.color = standardColor;
                break;
        }
    }
    
    public void ChangePhysicalState(physicalStates _newState)
    {
        if (_newState == physicalState)
            return;

        physicalState = _newState;
        
        switch (_newState)
        {
            case physicalStates.Attached:
                break;
            case physicalStates.Falling:
                break;
            case physicalStates.Catchable:
                break;
            case physicalStates.Grounded:
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (physicalState != physicalStates.Attached)
        {
            ChangePhysicalState(physicalStates.Grounded);
            Instantiate(objectCollisionEffect, other.GetContact(0).point, quaternion.identity);
        }
    }
}
