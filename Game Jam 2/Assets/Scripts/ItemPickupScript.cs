using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickupScript : MonoBehaviour
{
    
    [SerializeField] Transform lookDir; //Transfrom info for the camera
    [SerializeField] Transform PickupLoc; //Transform info for the location picked up objects will move to
    private float click; //Current input of the mouse left click
    GameObject lookObj; //Object the player is currently looking at
    

    void Update()
    {
        click = Input.GetAxis("Fire1");
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
        if (click != 0 && lookObj != null && lookObj.tag == "Pickupable") {
            Debug.Log("Pickup!!");
        }
    }
}
