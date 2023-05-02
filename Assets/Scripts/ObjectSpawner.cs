using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

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
            Vector3 winZonePosition = RandomPointInZone(winSpawnZone.bounds, 1f);

            if (winZonePosition.magnitude > 1000f)
                winZonePosition = winSpawnZone.bounds.center;

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

    Vector3 RandomPointInZone(Bounds _bounds, float _radius = 1.5f)
    {
        Vector3 spawnPoint = RandomSpawnPoint(_bounds);

        // Check for collisions within an increasing sphere until a valid point is found
        int attempts = 0;
        while (attempts < maxPositionFindAttempts)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnPoint, _radius, LayerMask.NameToLayer("SpawnZone"));
            
            if (colliders.Length == 0)
            {
                return spawnPoint;
            }

            spawnPoint = RandomSpawnPoint(_bounds);

            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn point after " + maxPositionFindAttempts + " attempts.");
        
        return Vector3.positiveInfinity;
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
                
                if(spawnPoint.magnitude > 1000f)
                    continue;

                spawnedObjects.Add(Instantiate(spawnableObject.objectPrefab, spawnPoint, Quaternion.Euler(0, 0, Random.Range(0f, 360f))));

                yield return new WaitForSeconds(0.1f);
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
        Vector3 respawnPoint = RandomPointAvoidWinZones(objectSpawnZone.bounds);
        respawnPoint.y = objectSpawnZone.bounds.min.y;
        
        _object.transform.position = respawnPoint;
        _object.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    
    Vector3 RandomPointAvoidWinZones(Bounds _bounds)
    {
        WinZone[] winZones = FindObjectsOfType<WinZone>();
        List<float> winZoneXPositions = new List<float>();
        
        foreach (var winZone in winZones)
            winZoneXPositions.Add(winZone.transform.position.x);

        Vector3 spawnPoint = RandomSpawnPoint(_bounds);
        
        int attempts = 0;
        int winZonesSuccessful = 0;
        float tolerance = 0.8f;
        
        while (attempts < maxPositionFindAttempts)
        {
            foreach (var winZoneX in winZoneXPositions)
            {
                if (spawnPoint.x < winZoneX - tolerance || spawnPoint.x > winZoneX + tolerance)
                    winZonesSuccessful++;
            }

            if (winZonesSuccessful == winZoneXPositions.Count)
                return spawnPoint;
            
            winZonesSuccessful = 0;
            spawnPoint = RandomSpawnPoint(_bounds);

            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn point after " + maxPositionFindAttempts + " attempts.");
        return _bounds.center; // fallback to the center of the spawn zone if no valid point is found
    }
}
