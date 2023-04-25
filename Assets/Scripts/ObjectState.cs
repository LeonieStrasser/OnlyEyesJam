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
    [SerializeField] float edgeFadeinDuration = 3f;
    [SerializeField] float edgeFadeOutDuration = 2f;



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
    private Coroutine edgeFeedbackCoroutine;
    public AnimationCurve edgeFadinCurve;

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
                Debug.Log("CLOSE");
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
        feedbackCoroutine = StartCoroutine(LerpMaterialFloat("_AnimatedBaseTextureOpacity", 1, feedbackFadeInDuration, (visualState == visualStates.LookedAt || visualState == visualStates.CloseToAttach || visualState == visualStates.Attached)));
    }

    void SetNeutralFeedbackState()
    {
        //meshRenderer.material.SetFloat("_AnimatedBaseTextureOpacity", 0);

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }

        feedbackCoroutine = StartCoroutine(LerpMaterialFloat("_AnimatedBaseTextureOpacity", 0, feedbackFadeOutDuration, true));

        if (edgeFeedbackCoroutine != null)
        {
            StopCoroutine(edgeFeedbackCoroutine);
        }

        edgeFeedbackCoroutine = StartCoroutine(LerpMaterialFloat("_EmissionFillAmount", 0, edgeFadeOutDuration, true));
    }

    void SetCloseToAttachFeedback()
    {
        telekinesisChannelParticles.Play();
        edgeFeedbackCoroutine = StartCoroutine(AnimateMaterialFloat("_EmissionFillAmount", 1, edgeFadeinDuration, (visualState == visualStates.CloseToAttach || visualState == visualStates.Attached), edgeFadinCurve));
    }





    IEnumerator LerpMaterialFloat(string _valueName, float _endValue, float _duration, bool _condition)
    {
        // Cache the ID of the material property
        int _propertyID = Shader.PropertyToID(_valueName);

        float _currentFeedbackValue = meshRenderer.material.GetFloat(_valueName);
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

    IEnumerator AnimateMaterialFloat(string _valueName, float _endValue, float _duration, bool _condition, AnimationCurve _curve)
    {
        // Cache the ID of the material property
        int _propertyID = Shader.PropertyToID(_valueName);

        float _currentFeedbackValue = meshRenderer.material.GetFloat(_valueName);
        float timer = 0;
        while (timer < _duration && _condition)
        {
            timer += Time.deltaTime;

            // Calculate the float value to set in the shader
            float _t = timer / _duration;
            float _curveValue = _curve.Evaluate(_t);
            float _value = Mathf.Lerp(_currentFeedbackValue, _endValue, _curveValue);


            _value = Mathf.Clamp01(_value);

            meshRenderer.material.SetFloat(_propertyID, _value);

            yield return null;
        }
    }
}
