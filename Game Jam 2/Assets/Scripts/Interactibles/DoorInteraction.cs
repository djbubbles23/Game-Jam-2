using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("How many keys are required to open the door.")]
    public int totalKeysRequired = 3;

    [Tooltip("Key to press for opening the door.")]
    public KeyCode openKey = KeyCode.O;

    [Header("References")]
    [Tooltip("Reference to the KeyManager.")]
    public KeyManager keyManager;

    private bool playerInZone = false;
    private Transform playerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerTransform = other.transform;

            // Show a prompt to open the door
            HUDManager.Instance.ShowActionPrompt("Press O to open door");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerTransform = null;

            // Hide the prompt when leaving the zone
            HUDManager.Instance.HideActionPrompt();
        }
    }

    private void Update()
    {
        // Check if player is in zone and presses the open key
        if (playerInZone && Input.GetKeyDown(openKey))
        {
            if (keyManager == null)
            {
                Debug.LogWarning("DoorInteraction: KeyManager not assigned.");
                return;
            }

            int collectedKeys = keyManager.keysCollected;
            if (collectedKeys < totalKeysRequired)
            {
                // Not enough keys
                int needed = totalKeysRequired - collectedKeys;
                HUDManager.Instance.ShowActionPrompt(
                    "Need " + needed + " more key(s) to open the door!"
                );
            }
            else
            {
                // Enough keys => open door (or end the game)
                Debug.Log("Door opened! You have enough keys. You win!");

                // 1. Hide prompt
                HUDManager.Instance.HideActionPrompt();

                // 2. Optionally play an animation or disable the door
                //    e.g., gameObject.SetActive(false);

                // 3. Tell the GameManager you’ve won
                GameManager.Instance.WinGame();
            }
        }
    }
}