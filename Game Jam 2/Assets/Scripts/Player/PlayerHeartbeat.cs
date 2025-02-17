using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHeartbeat : MonoBehaviour
{
    [SerializeField] private Transform dollTransform;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxDistance = 8f; // Distance at which the heartbeat starts to increase
    [SerializeField] private float floorThreshold = 1.0f;  // Maximum y difference to be considered on the same floor
    [SerializeField] private AudioSource heartbeatAudioSource;

    void Start()
    {
        heartbeatAudioSource.loop = true;
        heartbeatAudioSource.Play();
    }

    void Update()
    {
        if (dollTransform == null) return;

        if (Mathf.Abs(transform.position.y - dollTransform.position.y) > floorThreshold)
        {
            heartbeatAudioSource.pitch = minPitch;
            return;
        }

        float distance = Vector3.Distance(transform.position, dollTransform.position);

        // Calculate pitch based on distance
        float t = Mathf.Clamp01(1 - (distance / maxDistance));
        float newPitch = Mathf.Lerp(minPitch, maxPitch, t);

        heartbeatAudioSource.pitch = newPitch;
    }
}