using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickupScript : MonoBehaviour
{
    
    [SerializeField] Transform lookDir; //Transfrom info for the camera
    [SerializeField] Transform PickupLoc; //Transform info for the location picked up objects will move to
    private float click; //Current input of the mouse left click
    private bool escape; //Current input for the escape key
    GameObject lookObj; //Object the player is currently looking at
    Transform pickupPosStart; //Original position of the pickup Item

    void Start()
    {
        
    }
    void Update()
    {
        click = Input.GetAxis("Fire1");
        escape = Input.GetKey("escape") || Input.GetKey("tab");
        RaycastHit hit;
        if (Physics.Raycast (lookDir.position,lookDir.TransformDirection(Vector3.forward), out hit, 10)) {
            if (lookObj != null && lookObj != hit.transform.gameObject && lookObj.tag == "Pickupable") {
                lookObj.GetComponent<Outline>().OutlineWidth = 0;
            }

            if (hit.transform.gameObject.tag == "Pickupable") {
                lookObj = hit.transform.gameObject;
                lookObj.GetComponent<Outline>().OutlineWidth = 5;
            }
        }
    }
    void FixedUpdate()
    {
        if (lookObj != null && lookObj.tag == "Pickupable") {
            if (click != 0) {
                Debug.Log("Pickup!!");
                lookObj.GetComponent<ItemMoveScript>().MoveObj(PickupLoc);
                gameObject.GetComponent<FirstPersonController>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
            }
            if (escape) {
                Debug.Log("Put Down");
                lookObj.GetComponent<ItemMoveScript>().PutDown();
                gameObject.GetComponent<FirstPersonController>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
