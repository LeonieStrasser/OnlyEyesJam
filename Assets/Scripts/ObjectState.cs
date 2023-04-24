using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ObjectState : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject objectCollisionEffect;
    [SerializeField] ParticleSystem telekinesisChannelParticles;
    [SerializeField] float feedbackFadeInDuration = 4f;
    [SerializeField] float feedbackFadeOutDuration = 1f;



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

    private Coroutine feedbackCoroutine;


    //Color standardColor;

    void Start()
    {
        // standardColor = meshRenderer.material.color;
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
                SetChangeFeedback();
                break;
            case visualStates.CloseToAttach:
                SetCloseToAttachFeedback();
                break;
            case visualStates.Attached:
                meshRenderer.material.color = Color.green;
                break;
            case visualStates.Neutral:
                SetNeutralFeedbackState();
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

    void SetChangeFeedback()
    {
        feedbackCoroutine = StartCoroutine(SetMaterialFloat("_AnimatedBaseTextureOpacity", 1, feedbackFadeInDuration, (visualState == visualStates.LookedAt || visualState == visualStates.CloseToAttach || visualState == visualStates.Attached)));
    }

    void SetNeutralFeedbackState()
    {
        //meshRenderer.material.SetFloat("_AnimatedBaseTextureOpacity", 0);

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }

        feedbackCoroutine = StartCoroutine(SetMaterialFloat("_AnimatedBaseTextureOpacity", 0, feedbackFadeOutDuration, true));
    }

    void SetCloseToAttachFeedback()
    {
        telekinesisChannelParticles.Play();

    }





    IEnumerator SetMaterialFloat(string _valueName, float _endValue, float _duration, bool _condition)
    {
        // Cache the ID of the material property
        int _propertyID = Shader.PropertyToID(_valueName);

        float _currentFeedbackValue = meshRenderer.material.GetFloat("_AnimatedBaseTextureOpacity");
        float timer = 0;
        while (timer < _duration && _condition)
        {
            timer += Time.deltaTime;

            // Calculate the float value to set in the shader
            float _value = Mathf.Lerp(_currentFeedbackValue, _endValue, timer / _duration);
            _value = Mathf.Clamp01(_value);

            meshRenderer.material.SetFloat(_propertyID, _value);

            yield return null;
        }
    }
}
