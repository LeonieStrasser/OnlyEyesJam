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

        ObjectState objectState = other.gameObject.GetComponent<ObjectState>();
        if (objectState != null)
            AudioManager.instance.Play("Stone in Water");

    }
}
