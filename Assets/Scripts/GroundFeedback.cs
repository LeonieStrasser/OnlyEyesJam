using Unity.Mathematics;
using UnityEngine;

public class GroundFeedback : MonoBehaviour
{
    [SerializeField] GameObject groundFeedback;
    void OnCollisionEnter(Collision other)
    {
        ContactPoint[] contacts = other.contacts;

        Vector3 middlePos = Vector3.zero;

        for (int i = 0; i < contacts.Length; i++)
        {
            middlePos += contacts[i].point;
        }

        middlePos /= contacts.Length;

        Instantiate(groundFeedback, middlePos, quaternion.identity);
        AudioManager.instance.Play("Stone on Gravel");
    }
}
