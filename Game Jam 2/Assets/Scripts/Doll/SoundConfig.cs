using UnityEngine;

[System.Serializable]
public class SoundConfig
{
    public AudioSource audioSource;     // The AudioSource the doll should monitor
    public float detectionThreshold = 0.1f; // Minimum volume for detection
    public float priority = 1.0f;       // Higher means more important
}