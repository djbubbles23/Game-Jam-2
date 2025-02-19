using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false; // Set this to true to open the door
    public float openAngle = 90f; // Angle to open the door
    public float closedAngle = 0f; // Angle when door is closed
    public float rotationSpeed = 2f; // Speed of rotation

    private Quaternion targetRotation;
    private bool hasPlayed = false; 

    // SFX
    //public AudioSource audioSource;

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();

        targetRotation = Quaternion.Euler(0, closedAngle, 0);
    }

    void Update()
    {
        
        targetRotation = Quaternion.Euler(0, isOpen ? openAngle : closedAngle, 0);

        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        
        if (isOpen && !hasPlayed)
        {
            //audioSource.Play();
            hasPlayed = true; 
        }
        else if (!isOpen)
        {
            hasPlayed = false; 
        }
    }

    
    public void ToggleDoor()
    {
        isOpen = !isOpen;
        hasPlayed = false; 
    }
}
