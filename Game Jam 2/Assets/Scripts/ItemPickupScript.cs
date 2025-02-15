using System;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickupScript : MonoBehaviour
{
    
    [SerializeField] Transform lookDir; //Transfrom info for the camera
    [SerializeField] Transform PickupLoc; //Transform info for the location picked up objects will move to
    private float click; //Current input of the mouse left click
    GameObject pickupObj;
    

    void Update()
    {
        click = Input.GetAxis("Fire1");
    }
    void FixedUpdate()
    {
        if (click != 0) {
            RaycastHit hit;
            if (Physics.Raycast (lookDir.position,lookDir.TransformDirection(Vector3.forward), out hit, 10)) {
                Debug.Log(hit);
                pickupObj = hit.transform.gameObject;
                if (pickupObj.tag == "Pickupable") {
                    Debug.Log("Pickupable");
                }
            }
        }
    }
}
