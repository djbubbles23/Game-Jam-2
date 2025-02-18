using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CabinetHideZone : MonoBehaviour
{
    [Header("Hide Settings")]
    [Tooltip("Where the player gets teleported to when hiding.")]
    public Transform hidePosition;

    [Tooltip("Where the player gets teleported to when exiting the hide state.")]
    public Transform exitPosition;

    [Tooltip("Key to press for toggling hide/exit.")]
    public KeyCode hideKey = KeyCode.H;

    private bool playerInZone = false;
    private Transform playerTransform;

    // Flag to track whether the player is currently hidden
    private bool isHidden = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            playerTransform = other.transform;
            // Update UI prompt based on hidden state
            if (!isHidden)
            {
                HUDManager.Instance.ShowActionPrompt("Press H to hide");
            }
            else
            {
                HUDManager.Instance.ShowActionPrompt("Press H to exit");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            playerTransform = null;
            HUDManager.Instance.HideActionPrompt();
        }
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(hideKey))
        {
            if (!isHidden)
            {
                // Teleport player inside the cabinet and mark as hidden
                if (hidePosition != null)
                {
                    playerTransform.SetPositionAndRotation(hidePosition.position, hidePosition.rotation);
                }
                isHidden = true;
                Debug.Log("Player has been teleported inside and is now hidden.");
                HUDManager.Instance.ShowActionPrompt("Press H to exit");
            }
            else
            {
                // Teleport player to the exit position and mark as not hidden
                if (exitPosition != null)
                {
                    playerTransform.SetPositionAndRotation(exitPosition.position, exitPosition.rotation);
                }
                isHidden = false;
                Debug.Log("Player has been teleported out and is no longer hidden.");
                HUDManager.Instance.ShowActionPrompt("Press H to hide");
            }
        }
    }
}