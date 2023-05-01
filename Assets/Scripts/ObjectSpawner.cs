using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] List<SpawnableObject> spawnableObjects;
    [SerializeField] BoxCollider objectSpawnZone;
    [SerializeField] BoxCollider winSpawnZone;

    [SerializeField] GameObject winZonePrefab;
    [SerializeField] float maxCombinedWinZoneHeight = 3;
    
    [SerializeField] int maxPositionFindAttempts = 500;

    [MinMaxSlider(1, 5)] [SerializeField] Vector2Int winZoneAmount = Vector2Int.one;
    List<GameObject> spawnedObjects = new List<GameObject>();
    
    void Start()
    {
        StartCoroutine(FirstSpawning());
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnObjects());
    }

    IEnumerator FirstSpawning()
    {
        StartSpawning();

        yield return new WaitForSeconds(4f);
        
        PlaceWinZones();
    }

    public void PlaceWinZones()
    {
        float addedHeight = 0;
        
        for (int i = 0; i < Random.Range(winZoneAmount.x, winZoneAmount.y + 1); i++)
        {
            Vector3 winZonePosition = RandomPointInZone(winSpawnZone.bounds, 2.5f);

            if (addedHeight + winZonePosition.y > maxCombinedWinZoneHeight)
            {
                winZonePosition.y = Random.Range(0, maxCombinedWinZoneHeight - addedHeight);
            }
            
            addedHeight += winZonePosition.y;
            
            GameObject newWinZone = Instantiate(winZonePrefab, winZonePosition, Quaternion.identity);
            newWinZone.transform.localScale = Vector3.zero;
            newWinZone.transform.DOScale(Vector3.one, 1f);
        }
        
        LevelManager.instance.RegisterWinZones();
    }

    Vector3 RandomPointInZone( Bounds _bounds, float radius = 1.5f)
    {
        var bounds = objectSpawnZone.bounds;

        Vector3 spawnPoint = RandomSpawnPoint(_bounds);

        // Check for collisions within an increasing sphere until a valid point is found
        int attempts = 0;
        while (attempts < maxPositionFindAttempts)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnPoint, radius, LayerMask.NameToLayer("SpawnZone"));
            
            if (colliders.Length == 0)
            {
                return spawnPoint;
            }

            spawnPoint = RandomSpawnPoint(_bounds);
            
            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn point after " + maxPositionFindAttempts + " attempts.");
        return bounds.center; // fallback to the center of the spawn zone if no valid point is found
    }

    Vector3 RandomSpawnPoint(Bounds _bounds)
    {
        return new Vector3
        (
            Random.Range(_bounds.min.x, _bounds.max.x),
            Random.Range(_bounds.min.y, _bounds.max.y),
            Random.Range(_bounds.min.z, _bounds.max.z)
        );
    }

    IEnumerator SpawnObjects()
    {
        foreach (var spawnableObject in spawnableObjects)
        {
            for (int i = 0; i < Random.Range(spawnableObject.objectAmount.x, spawnableObject.objectAmount.y + 1); i++)
            {
                Vector3 spawnPoint = RandomPointInZone(objectSpawnZone.bounds);

                spawnedObjects.Add(Instantiate(spawnableObject.objectPrefab, spawnPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f))));

                yield return null;
            }
        }
    }
    
    public void ClearAllObjects()
    {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            Destroy(spawnedObjects[i]);
            spawnedObjects.RemoveAt(i);
        }
    }

    public void RespawnSpecificObject(GameObject _object)
    {
        _object.transform.position = RandomPointInZone(objectSpawnZone.bounds);
        _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
