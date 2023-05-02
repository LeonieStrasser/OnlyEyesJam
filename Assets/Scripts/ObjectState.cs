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
    
    [SerializeField] float feedbackFadeOutDuration = 1f;
    [SerializeField] float edgeFadeinDuration = 3f;
    [SerializeField] float edgeFadeOutDuration = 1f;
    
    float feedbackFadeInDuration;
    float boxCastWidth, boxCastHeight;
    
    Rigidbody rb;

    [SerializeField] Material debugMaterial;
    
    public enum physicalStates
    {
        Grounded, Falling, Catchable, Attached, Immovable
    }

    public physicalStates physicalState;

    public enum visualStates
    {
        Neutral, LookedAt, CloseToAttach, Attached, WinGroup
    }

    public visualStates visualState;

    private Coroutine feedbackCoroutine;
    private Coroutine edgeFeedbackCoroutine;
    private Coroutine edgeFadeoutFeedbackCoroutine;
    public AnimationCurve edgeFadinCurve;
    public AnimationCurve edgeFadOutCurve;
    
    Vector3 castPos;


    CubeGroupFeedback myGroupFeedback;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        visualState = visualStates.Neutral;
        physicalState = physicalStates.Falling;

        feedbackFadeInDuration = GazeManager.Instance.timeTillTelekinesis;

        myGroupFeedback = FindObjectOfType<CubeGroupFeedback>();
    }

    void Update()
    {
        if(physicalState == physicalStates.Attached)
            UnderneathCheck();

        if(physicalState == physicalStates.Falling && rb.velocity.magnitude < 0.1f)
            ChangePhysicalState(physicalStates.Grounded);

        if(visualState == visualStates.WinGroup)
        {
            meshRenderer.material.SetFloat("_RainWorldBorder", myGroupFeedback.RainWorldBorder);
        }
    }

    void UnderneathCheck()
    {
        Bounds bounds = meshRenderer.bounds;
        

        boxCastWidth = bounds.size.x * 0.75f;
        boxCastHeight = 0.25f;

        //castPos = new Vector3(transform.position.x, transform.position.y - bounds.extents.y);
        castPos = new Vector3(bounds.min.x + (bounds.max.x - bounds.min.x) * 0.5f, bounds.min.y);
        
        hasSomethingUnderneath = Physics.BoxCast(castPos, new Vector3(boxCastWidth, boxCastHeight, 1), Vector3.down);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(castPos, new Vector3(boxCastWidth, boxCastHeight, 1));
    }

    public void ChangeVisualState(visualStates _newState)
    {
        visualStates _oldState = visualState;

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
                if (_oldState == visualStates.Attached)
                    StartCoroutine(TelekinesisSoundChangeDown());
                SetAttachFeedback();
                break;
            case visualStates.Neutral:
                SetNeutralFeedbackState();
                telekinesisChannelParticles.Stop();
                break;
            case visualStates.WinGroup:
                SetWinFeedback();
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

        ObjectState objectState = other.gameObject.GetComponent<ObjectState>();
        if (objectState != null)
            AudioManager.instance.Play("Stone on Stone");

        GroundFeedback groundFeedback = other.gameObject.GetComponent<GroundFeedback>();
        if (groundFeedback != null)
            AudioManager.instance.Play("Stone on Gravel");

        if (physicalState != physicalStates.Attached)
        {
            ChangePhysicalState(physicalStates.Grounded);
            Instantiate(objectCollisionEffect, other.GetContact(0).point, quaternion.identity);
        }
    }
    Coroutine lastRoutine;
    void SetChangeFeedback()
    {
        lastRoutine = StartCoroutine(TelekinesisSound(feedbackFadeInDuration));
        feedbackCoroutine = StartCoroutine(LerpMaterialFloat("_AnimatedBaseTextureOpacity", 1, feedbackFadeInDuration, (visualState == visualStates.LookedAt || visualState == visualStates.CloseToAttach || visualState == visualStates.Attached)));
    }

    void SetAttachFeedback()
    {
        AudioManager.instance.Play("TKinese Static 2");

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

        TelekinesisSoundStop();

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

    void SetWinFeedback()
    {
        //meshRenderer.material = debugMaterial;
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

    IEnumerator TelekinesisSound(float _duration)
    {
        bool upPlaying = false;

        AudioManager.instance.Play("TKinese Static 1");

        float timer = 0;
        while (timer < (_duration - AudioManager.instance.timeBetweenWindupSoundAndTelekinesis + AudioManager.instance.GetLength("TKinese State Change Up")))
        {
            timer += Time.deltaTime;

            if (timer >= _duration - AudioManager.instance.timeBetweenWindupSoundAndTelekinesis)
            {
                if (!upPlaying)
                {
                    AudioManager.instance.Play("TKinese State Change Up");
                    upPlaying = true;
                }
            }

            yield return null;

        }

        if (upPlaying)
        {
            AudioManager.instance.Stop("TKinese State Change Up");
        }

    }
    public void TelekinesisSoundStop()
    {
        StopCoroutine(lastRoutine);
        AudioManager.instance.Stop("TKinese Static 1");
        AudioManager.instance.Stop("TKinese Static 2");
        AudioManager.instance.Stop("TKinese State Change Up");
    }
    public IEnumerator TelekinesisSoundChangeDown()
    {
        AudioManager.instance.Play("TKinese State Change Down");

        yield return new WaitForSeconds(AudioManager.instance.GetLength("TKinese State Change Down"));

        AudioManager.instance.Stop("TKinese State Change Down");

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
