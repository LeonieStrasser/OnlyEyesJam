using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CubeGroupFeedback : MonoBehaviour
{
    [OnValueChanged("SetRainBorderInAllMaterials")] [SerializeField] float rainWorldBorder = -50;
    public float RainWorldBorder
    {
        set
        {
            rainWorldBorder = value;

        }

        get
        {
            return rainWorldBorder;
        }
    }

    public float maxWorldValue;
    public float minWorldValue;
    public float winEffectDuration;

    public Material[] allMovableMaterials;









    public void StartWingroupsEffect()
    {

        StartCoroutine(LerpWorldFloat(minWorldValue, maxWorldValue, winEffectDuration));
    }



    IEnumerator LerpWorldFloat(float _startValue, float _endValue, float _duration)
    {



        float timer = 0;
        while (timer < _duration)
        {
            timer += Time.deltaTime;

            // Calculate the float value to set 
            float _value = Mathf.Lerp(_startValue, _endValue, timer / _duration);
            RainWorldBorder = _value;


            yield return null;
        }
    }


















    // --------- EDITOR-----------



    void SetRainBorderInAllMaterials()
    {
#if UNITY_EDITOR
        Debug.Log("Value changed");

        foreach (var mat in allMovableMaterials)
        {
            mat.SetFloat("_RainWorldBorder", rainWorldBorder);
        }
#endif
    }

    //----------------------------

}
