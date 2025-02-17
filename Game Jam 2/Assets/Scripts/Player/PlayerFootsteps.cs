using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;
    // The time between footsteps. Adjust based on movement speed.

    [SerializeField] private AudioSource footstepAudioSource;
    private float footstepTimer;
    private Rigidbody playerRigidbody;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float playerSpeed = playerRigidbody.linearVelocity.magnitude / 2.0f;

        float currentInterval = playerSpeed > 3.0f ? 0.3f : footstepInterval;

        if (playerSpeed > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= currentInterval)
            {
                PlayRandomFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void PlayRandomFootstep()
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        footstepAudioSource.PlayOneShot(clip);
    }
}