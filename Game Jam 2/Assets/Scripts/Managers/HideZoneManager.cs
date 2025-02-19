using UnityEngine;

public class HideZoneManager : MonoBehaviour
{
    public static HideZoneManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("AudioSource for cabinet open sound (when player hides).")]
    public AudioSource openSound;
    [Tooltip("AudioSource for cabinet close sound (when player exits hiding).")]
    public AudioSource closeSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionally: DontDestroyOnLoad(gameObject);
    }

    public void PlayOpenSound()
    {
        if (openSound != null)
            openSound.Play();
    }

    public void PlayCloseSound()
    {
        if (closeSound != null)
            closeSound.Play();
    }
}