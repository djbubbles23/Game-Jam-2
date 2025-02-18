using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI actionPrompt;

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
}