using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] List<SpawnableObject> spawnableObjects;
    [SerializeField] BoxCollider spawnZone;
    
    void Start()
    {
        StartCoroutine(SpawnObjects());
    }
    
    Vector3 RandomPointInBounds(Bounds _bounds)
    {
        return new Vector3(
            Random.Range(_bounds.min.x, _bounds.max.x),
            Random.Range(_bounds.min.y, _bounds.max.y),
            Random.Range(_bounds.min.z, _bounds.max.z)
        );
    }

    IEnumerator SpawnObjects()
    {
        float lastSpawnX = 0;
        
        foreach (var spawnableObject in spawnableObjects)
        {
            for (int i = 0; i < Random.Range(spawnableObject.objectAmount.x, spawnableObject.objectAmount.y + 1); i++)
            {
                Vector3 spawnPoint = RandomPointInBounds(spawnZone.bounds);

                while (spawnPoint.x > lastSpawnX - 3 && spawnPoint.x < lastSpawnX + 3)
                {
                    spawnPoint = RandomPointInBounds(spawnZone.bounds);
                }

                lastSpawnX = spawnPoint.x;
                
                Instantiate(spawnableObject.objectPrefab, spawnPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
