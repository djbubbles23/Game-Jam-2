using UnityEngine;

public class DollWhispers : MonoBehaviour
{
    [SerializeField] private AudioSource whisperAudioSource;
    [SerializeField] private AudioClip[] whisperClips;
    [SerializeField] private float minInterval = 10f;
    [SerializeField] private float maxInterval = 15f;

    private float timer;
    private float nextWhisperTime;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextWhisperTime)
        {
            PlayRandomWhisper();
            ResetTimer();
        }
    }

    private void PlayRandomWhisper()
    {
        if (whisperClips.Length == 0)
            return;
        AudioClip clip = whisperClips[Random.Range(0, whisperClips.Length)];
        whisperAudioSource.PlayOneShot(clip);
    }

    private void ResetTimer()
    {
        timer = 0f;
        nextWhisperTime = Random.Range(minInterval, maxInterval);
    }
}