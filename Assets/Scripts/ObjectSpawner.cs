using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] List<SpawnableObject> spawnableObjects;
    [SerializeField] BoxCollider spawnZone;

    [SerializeField] int maxPositionFindAttempts = 500;
    
    void Start()
    {
        StartCoroutine(SpawnObjects());
    }

    Vector3 RandomPointInZone(float radius = 1.5f)
    {
        var bounds = spawnZone.bounds;

        Vector3 spawnPoint = RandomSpawnPoint();

        // Check for collisions within an increasing sphere until a valid point is found
        int attempts = 0;
        while (attempts < maxPositionFindAttempts)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnPoint, radius, LayerMask.NameToLayer("SpawnZone"));
            
            if (colliders.Length == 0)
            {
                return spawnPoint;
            }

            foreach (var coll in colliders)
            {
                Debug.Log("Found Collider: " + coll.gameObject.name);
            }

            spawnPoint = RandomSpawnPoint();
            
            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn point after " + maxPositionFindAttempts + " attempts.");
        return bounds.center; // fallback to the center of the spawn zone if no valid point is found
    }

    Vector3 RandomSpawnPoint()
    {
        Bounds bounds = spawnZone.bounds;
        
        return new Vector3
        (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    IEnumerator SpawnObjects()
    {
        foreach (var spawnableObject in spawnableObjects)
        {
            for (int i = 0; i < Random.Range(spawnableObject.objectAmount.x, spawnableObject.objectAmount.y + 1); i++)
            {
                Vector3 spawnPoint = RandomPointInZone();

                Instantiate(spawnableObject.objectPrefab, spawnPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

                yield return null;
            }
        }
    }

    public void RespawnSpecificObject(GameObject _object)
    {
        _object.transform.position = RandomPointInZone();
        _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
