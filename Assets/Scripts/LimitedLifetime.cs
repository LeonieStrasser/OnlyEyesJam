using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifetime : MonoBehaviour
{
    [SerializeField] float lifetime = 3f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
