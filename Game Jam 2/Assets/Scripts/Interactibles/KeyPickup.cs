using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    private bool playerInZone = false;
    private Transform playerTransform;

    // Use OnTriggerEnter to detect when the player enters the key's pickup zone.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerTransform = other.transform;
            // Show a prompt using your HUDManager
            HUDManager.Instance.ShowActionPrompt("Press K or Mouse Left Click to pick up key");
        }
    }

    // When the player leaves the zone, hide the prompt.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerTransform = null;
            HUDManager.Instance.HideActionPrompt();
        }
    }

    private void Update()
    {
        // If the player is in the zone and presses K, pick up the key.
        if (playerInZone &&
            (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown((int)MouseButton.Left)))
        {
            KeyManager km = FindFirstObjectByType<KeyManager>(); // Ensure your KeyManager is in the scene.
            if (km != null)
            {
                km.OnKeyCollected();
            }

            // Optionally show a quick prompt indicating the key was collected.
            HUDManager.Instance.ShowActionPrompt("Key collected!");

            // Destroy this key object.
            Destroy(gameObject);
        }
    }
}