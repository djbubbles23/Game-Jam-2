using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CabinetHideZone : MonoBehaviour
{
    [Header("Hide Settings")]
    [Tooltip("Where the player gets teleported when hiding.")]
    public Transform hidePosition;
    [Tooltip("Where the player gets teleported when exiting hiding.")]
    public Transform exitPosition;
    [Tooltip("Key to toggle hide/exit.")]
    public KeyCode hideKey = KeyCode.H;

    private bool playerInZone = false;
    private Transform playerTransform;

    // Track whether the player is currently hidden.
    private bool isHidden = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerTransform = other.transform;
            // Show prompt based on current state.
            if (!isHidden)
                HUDManager.Instance.ShowActionPrompt("Press H to hide");
            else
                HUDManager.Instance.ShowActionPrompt("Press H to exit");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerTransform = null;
            HUDManager.Instance.HideActionPrompt();
            // Optional: If the player exits while hidden, automatically exit hiding.
            if (isHidden)
            {
                if (exitPosition != null)
                {
                    other.transform.SetPositionAndRotation(exitPosition.position, exitPosition.rotation);
                }
                isHidden = false;
                HUDManager.Instance.HideHideOverlay();
                // Play close sound
                HideZoneManager.Instance.PlayCloseSound();
            }
        }
    }

    private void Update()
    {
        if (playerInZone && Input.GetKeyDown(hideKey))
        {
            if (!isHidden)
            {
                // Teleport the player into the cabinet and mark as hidden.
                if (hidePosition != null)
                {
                    playerTransform.SetPositionAndRotation(hidePosition.position, hidePosition.rotation);
                }
                isHidden = true;
                Debug.Log("Player is now hidden.");
                HUDManager.Instance.ShowActionPrompt("Press H to exit");
                HUDManager.Instance.ShowHideOverlay();
                // Play open sound
                HideZoneManager.Instance.PlayOpenSound();
            }
            else
            {
                // Teleport the player out and mark as not hidden.
                if (exitPosition != null)
                {
                    playerTransform.SetPositionAndRotation(exitPosition.position, exitPosition.rotation);
                }
                isHidden = false;
                Debug.Log("Player is no longer hidden.");
                HUDManager.Instance.ShowActionPrompt("Press H to hide");
                HUDManager.Instance.HideHideOverlay();
                // Play close sound
                HideZoneManager.Instance.PlayCloseSound();
            }
        }
    }
}