using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DollEerieSound : MonoBehaviour
{
    [SerializeField] private AudioSource eerieAudioSource;
    [SerializeField] private AudioClip eerieLoopClip;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float fadeSpeed = 1.0f;
    [SerializeField] private float triggerDistance = 5f;

    private void Start()
    {
        eerieAudioSource.clip = eerieLoopClip;
        eerieAudioSource.loop = true;
        eerieAudioSource.Play();
        eerieAudioSource.volume = 0f;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float targetVolume = distance <= triggerDistance ? maxVolume : 0f;

        eerieAudioSource.volume = Mathf.MoveTowards(
            eerieAudioSource.volume,
            targetVolume,
            fadeSpeed * Time.deltaTime
        );
    }
}