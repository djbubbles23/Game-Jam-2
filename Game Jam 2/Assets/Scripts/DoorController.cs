using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false; // Set this to true to open the door
    public float openAngle = 90f; // Angle to open the door
    public float closedAngle = 0f; // Angle when door is closed
    public float rotationSpeed = 2f; // Speed of rotation

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = Quaternion.Euler(0, closedAngle, 0);
    }

    void Update()
    {
        // Determine the target rotation based on isOpen
        targetRotation = Quaternion.Euler(0, isOpen ? openAngle : closedAngle, 0);

        // Smoothly rotate the door
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Call this function to toggle the door state
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
