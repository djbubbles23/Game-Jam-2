using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI actionPrompt;
    [SerializeField] private Image hideOverlay; // Full-screen black Image

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowActionPrompt(string message)
    {
        actionPrompt.text = message;
        actionPrompt.gameObject.SetActive(true);
    }

    public void HideActionPrompt()
    {
        actionPrompt.gameObject.SetActive(false);
    }

    public void ShowHideOverlay()
    {
        if (hideOverlay != null)
        {
            hideOverlay.gameObject.SetActive(true);
        }
    }

    public void HideHideOverlay()
    {
        if (hideOverlay != null)
        {
            hideOverlay.gameObject.SetActive(false);
        }
    }
}