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
    
    Vector3 RandomPointInZone()
    {
        var bounds = spawnZone.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    IEnumerator SpawnObjects()
    {
        float lastSpawnX = 0;
        
        foreach (var spawnableObject in spawnableObjects)
        {
            for (int i = 0; i < Random.Range(spawnableObject.objectAmount.x, spawnableObject.objectAmount.y + 1); i++)
            {
                Vector3 spawnPoint = RandomPointInZone();

                while (spawnPoint.x > lastSpawnX - 3 && spawnPoint.x < lastSpawnX + 3)
                {
                    spawnPoint = RandomPointInZone();
                }

                lastSpawnX = spawnPoint.x;
                
                Instantiate(spawnableObject.objectPrefab, spawnPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void RespawnSpecificObject(GameObject _object)
    {
        _object.transform.position = RandomPointInZone();
        _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
