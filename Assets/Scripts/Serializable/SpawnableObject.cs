using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class SpawnableObject
{
    public GameObject objectPrefab;
    [MinMaxSlider(1, 10)]
    public Vector2Int objectAmount = Vector2Int.one;
}
