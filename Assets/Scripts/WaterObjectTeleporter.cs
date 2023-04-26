using UnityEngine;

public class WaterObjectTeleporter : MonoBehaviour
{
    ObjectSpawner spawner;
    void Start()
    {
        spawner = FindObjectOfType<ObjectSpawner>();
    }

    void OnTriggerEnter(Collider other)
    {
        spawner.RespawnSpecificObject(other.gameObject);
    }
}
