using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHeartbeat : MonoBehaviour
{
    [SerializeField] private Transform dollTransform;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float minPitch = 1.0f;
    [SerializeField] private float maxDistance = 8f; // Distance at which the heartbeat is fastest

    [SerializeField] private AudioSource heartbeatAudioSource;

    void Start()
    {
        heartbeatAudioSource.loop = true;
        heartbeatAudioSource.Play();
    }

    void Update()
    {
        if (dollTransform == null) return;

        float distance = Vector3.Distance(transform.position, dollTransform.position);

        // Calculate pitch based on distance
        float t = Mathf.Clamp01(1 - (distance / maxDistance));
        float newPitch = Mathf.Lerp(minPitch, maxPitch, t);

        heartbeatAudioSource.pitch = newPitch;
        heartbeatAudioSource.volume = 0.5f;
    }
}