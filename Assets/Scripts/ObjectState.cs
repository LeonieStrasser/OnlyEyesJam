using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class ObjectState : MonoBehaviour
{
    [HideInInspector] public bool hasSomethingUnderneath;
    
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject objectCollisionEffect;
    [SerializeField] GameObject glowingOrb;
    [SerializeField] GameObject impulseSpherePrefab;
    [SerializeField] ParticleSystem telekinesisChannelParticles;
    float feedbackFadeInDuration;
    [SerializeField] float feedbackFadeOutDuration = 1f;
    [SerializeField] float edgeFadeinDuration = 3f;
    [SerializeField] float edgeFadeOutDuration = 1f;

    Rigidbody rb;
    
    public enum physicalStates
    {
        Grounded, Falling, Catchable, Attached, Immovable
    }

    public physicalStates physicalState;

    public enum visualStates
    {
        Neutral, LookedAt, CloseToAttach, Attached
    }

    public visualStates visualState;

    private Coroutine feedbackCoroutine;
    private Coroutine edgeFeedbackCoroutine;
    private Coroutine edgeFadeoutFeedbackCoroutine;
    public AnimationCurve edgeFadinCurve;
    public AnimationCurve edgeFadOutCurve;

    //Color standardColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        visualState = visualStates.Neutral;
        physicalState = physicalStates.Falling;

        feedbackFadeInDuration = GazeManager.Instance.timeTillTelekinesis;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * 1.3f, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, 1.3f))
        {
            hasSomethingUnderneath = true;
        }
        else
        {
            
        }
        
        if(physicalState == physicalStates.Falling && rb.velocity.magnitude < 0.1f)
            ChangePhysicalState(physicalStates.Grounded);
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
                SetAttachFeedback();
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
            case physicalStates.Immovable:
                break;
            case physicalStates.Grounded:
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(!(other.gameObject.CompareTag("MoveableObject") || other.gameObject.CompareTag("Ground")))
            return;
        
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

    void SetAttachFeedback()
    {
        glowingOrb.SetActive(true);

        if (edgeFadeoutFeedbackCoroutine != null)
        {
            StopCoroutine(edgeFadeoutFeedbackCoroutine);
        }
        edgeFadeoutFeedbackCoroutine = StartCoroutine(AnimateMaterialFloat("_EmissionFadeOutAmount", 1, GazeManager.Instance.telekinesisMaxDuration, (visualState == visualStates.Attached), edgeFadOutCurve));

        Instantiate(impulseSpherePrefab, this.transform.position, Quaternion.identity);
    }

    void SetNeutralFeedbackState()
    {
        //meshRenderer.material.SetFloat("_AnimatedBaseTextureOpacity", 0);
        
        glowingOrb.SetActive(false);

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
        meshRenderer.material.SetFloat("_EmissionFadeOutAmount", 0);
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
