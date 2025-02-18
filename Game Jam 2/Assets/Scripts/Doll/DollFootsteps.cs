using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
public class DollFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] dollFootstepClips;
    [SerializeField] private float footstepInterval = 0.6f;

    [SerializeField] private AudioSource footstepAudioSource;
    private NavMeshAgent navAgent;
    private float footstepTimer;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // The doll's speed
        float speed = navAgent.velocity.magnitude;

        if (speed > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
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
        if (dollFootstepClips.Length == 0) return;
        AudioClip clip = dollFootstepClips[Random.Range(0, dollFootstepClips.Length)];
        footstepAudioSource.PlayOneShot(clip);
    }
}