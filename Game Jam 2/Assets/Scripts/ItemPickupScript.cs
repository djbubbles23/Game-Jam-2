using Unity.VisualScripting;
using UnityEngine;

public class ItemPickupScript : MonoBehaviour
{
    
    [SerializeField] Transform lookDir; //Transfrom info for the camera
    [SerializeField] Transform PickupLoc; //Transform info for the location picked up objects will move to
    private Vector3 relPos;
    private float click; //Current input of the mouse left click
    private bool escape; //Current input for the escape key
    private GameObject lookObj; //Object the player is currently looking at
    private GameObject newObj;
    private Vector3 pickupPosStart; //Original position of the pickup Item
    private Quaternion pickupRotStart; //Oriiginal rotation of the pickup item
    private bool pickedUp = false; //Tracks if we have something picked up

    void Start()
    {
        relPos = lookDir.position - PickupLoc.position;
    }
    void Update()
    {
        click = Input.GetAxis("Fire1");
        escape = Input.GetKey("escape") || Input.GetKey("tab");
        
        RaycastHit hit;
        if (Physics.Raycast(lookDir.position,lookDir.TransformDirection(Vector3.forward), out hit, 5,3)) {
            newObj = hit.transform.gameObject;
            //Resets Outline of object we are no longer looking at
            if (lookObj != null && lookObj != newObj && lookObj.tag == "Pickupable") {
                lookObj.GetComponent<Outline>().OutlineWidth = 0;
            }

            //Ckecks that what we're looking at is pickupable
            if (newObj.tag == "Pickupable") {
                lookObj = newObj;
                lookObj.GetComponent<Outline>().OutlineWidth = 5;

                if (click != 0 && !pickedUp) {
                    Debug.Log("Pickup!!");
                    pickupPosStart = PickupLoc.position;
                    pickupRotStart = PickupLoc.rotation;
                    pickedUp = true;

                    lookObj.GetComponent<ItemMoveScript>().MoveObj(PickupLoc);

                    gameObject.GetComponent<FirstPersonController>().enabled = false;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        if (escape && pickedUp) {
            Debug.Log("Put Down");
            pickedUp = false;

            lookObj.GetComponent<ItemMoveScript>().PutDown();

            gameObject.GetComponent<FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;

            PickupLoc.position = pickupPosStart;
            PickupLoc.rotation = pickupRotStart;
        }
    }
    void FixedUpdate()
    {
        
    }
}
