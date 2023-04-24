using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class GazeIndicator : MonoBehaviour
{
    public static GazeIndicator Instance;
    
    [SerializeField] GameObject canvasSystemsParent;
    [SerializeField] GameObject worldSystemsParent;

    [MinMaxSlider(0.1f, 2f)]
    [SerializeField] Vector2 indicatorSize = new Vector2(0.1f, 1.5f);
    [SerializeField] float indicatorScaleSpeed = 1f;

    List<ParticleSystem> canvasSystems = new List<ParticleSystem>();
    List<ParticleSystem> worldSystems = new List<ParticleSystem>();

    bool isFocused = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvasSystems = canvasSystemsParent.GetComponentsInChildren<ParticleSystem>().ToList();
        worldSystems = worldSystemsParent.GetComponentsInChildren<ParticleSystem>().ToList();
        
        worldSystemsParent.transform.localScale = Vector3.one * indicatorSize.y;
        canvasSystemsParent.transform.localScale = Vector3.one * indicatorSize.y;
        
        PlayAll(worldSystems);
    }

    public void StartFocusAnim()
    {
        if (isFocused)
            return;
        
        isFocused = true;
        
        PlayAll(canvasSystems);
        worldSystemsParent.transform.DOScale(Vector3.one * indicatorSize.y, indicatorScaleSpeed);
        canvasSystemsParent.transform.DOScale(Vector3.one * indicatorSize.y, indicatorScaleSpeed);
    }
    
    public void EndFocusAnim()
    {
        if (!isFocused)
            return;
        
        isFocused = false;
        
        StopAll(canvasSystems);
        worldSystemsParent.transform.DOScale(Vector3.one * indicatorSize.x, indicatorScaleSpeed);
        canvasSystemsParent.transform.DOScale(Vector3.one * indicatorSize.x, indicatorScaleSpeed);
    }

    void PlayAll(List<ParticleSystem> _particleSystems)
    {
        foreach (var _pSystem in _particleSystems)
        {
            _pSystem.Play();
        }
    }
    
    void StopAll(List<ParticleSystem> _particleSystems)
    {
        foreach (var _pSystem in _particleSystems)
        {
            _pSystem.Stop();
        }
    }
    
}
