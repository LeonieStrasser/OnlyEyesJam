using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateColorValue : MonoBehaviour
{
    public AnimationCurve animCurve;

    public float duration;

    public MeshRenderer myRenderer;
    public string valueToAnimate;

    private void Start()
    {
        StartCoroutine(AnimateValueTOverTime());
    }

    IEnumerator AnimateValueTOverTime()
    {

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Calculate the float value to set in the shader
            float _t = timer / duration;
            float _curveValue = animCurve.Evaluate(_t);
            float _value = Mathf.Lerp(0, 1, _curveValue);


            myRenderer.material.SetFloat(valueToAnimate, _value);


           

            yield return null;
        }
    }
}
