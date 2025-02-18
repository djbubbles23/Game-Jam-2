using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float footstepInterval = 0.5f;
    // Multipliers for crouched state:
    [SerializeField] private float crouchedIntervalMultiplier = 1.5f;
    [SerializeField] private float crouchedFootstepsVolume = 0.5f;

    [SerializeField] private AudioSource footstepAudioSource;
    private float footstepTimer;
    private Rigidbody playerRigidbody;
    private FirstPersonController fpc;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        fpc = GetComponent<FirstPersonController>();  // Get the controller to check crouch state
    }

    void Update()
    {
        bool isCrouched = (fpc != null) && fpc.IsCrouched;
        float playerSpeed = playerRigidbody.linearVelocity.magnitude / 2.0f;

        // Determine the base interval based on speed.
        float currentInterval = playerSpeed > 3.0f ? 0.3f : footstepInterval;

        if (isCrouched)
        {
            currentInterval *= crouchedIntervalMultiplier;
        }

        if (playerSpeed > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= currentInterval)
            {
                PlayRandomFootstep(isCrouched);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    // Pass whether the player is crouched to adjust volume.
    private void PlayRandomFootstep(bool isCrouched)
    {
        if (footstepClips.Length == 0) return;
        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        float volume = isCrouched ? crouchedFootstepsVolume : 1.0f;
        footstepAudioSource.PlayOneShot(clip, volume);
    }
}