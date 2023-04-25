using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScale : MonoBehaviour
{
    public AnimationCurve animCurve;

    public float duration;
    public float maxScale;

    private void Start()
    {
        StartCoroutine(AnimateScaleTOverTime());
    }

    IEnumerator AnimateScaleTOverTime()
    {

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Calculate the float value to set in the shader
            float _t = timer / duration;
            float _curveValue = animCurve.Evaluate(_t);
            float _value = Mathf.Lerp(0, 1, _curveValue);


            _value = Mathf.Clamp01(_value);

            this.transform.localScale = new Vector3(_value * maxScale, _value * maxScale, _value * maxScale);

            yield return null;
        }
    }
}
