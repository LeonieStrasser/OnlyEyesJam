using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ObjectState : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject objectCollisionEffect;
    [SerializeField] ParticleSystem telekinesisChannelParticles;

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
                StartCoroutine(TelekinesisChargeFeedback(4f));
                break;
            case visualStates.CloseToAttach:
                telekinesisChannelParticles.Play();
                break;
            case visualStates.Attached:
                meshRenderer.material.color = Color.green;
                break;
            case visualStates.Neutral:
                meshRenderer.material.color = standardColor;
                telekinesisChannelParticles.Stop();
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

    IEnumerator TelekinesisChargeFeedback(float _duration)
    {
        float timer = 0;
        while (timer < _duration && visualState == visualStates.LookedAt)
        {
            timer += Time.deltaTime;
            
            meshRenderer.material.color = Color.Lerp(Color.white, Color.yellow, timer / _duration);

            yield return null;
        }
    }
}
