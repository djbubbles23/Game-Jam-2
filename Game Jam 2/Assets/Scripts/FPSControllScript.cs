using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSControllScript : MonoBehaviour
{
    [SerializeField] Transform cam;
    private Rigidbody rigid; 
    [SerializeField] float mouseSens = 2f;  //Mouse's sensitivity
    private float mouseXV;                  //Movement of the mouse's X this frame
    private float mouseYV;                  //Movement of the mouse's Y this frame
    private float camRotX = 0f;             //Current Rotation of the camera in the X axis
    private float camRotY = 0f;             //Current Rotation of the camera in the Y axis
    [SerializeField] float moveSpeed = 1f;  //The players movementspeed
    private float moveX;                    //Current input for movement in the X axis
    private float moveZ;                    //Current input for movement in the Z axis
    private Vector3 moveForce;                   //Current applied force for movement
    
    
        void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Inputs
        mouseXV = Input.GetAxis("Mouse X") * mouseSens;
        mouseYV = Input.GetAxis("Mouse Y") * mouseSens;
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        //Camera Movement handeling
        camRotX -= mouseXV;
        camRotY -= mouseYV;
        camRotY = Math.Clamp(camRotY, -90f, 90f);

        cam.localEulerAngles = Vector3.right * camRotY;
        transform.Rotate(Vector3.up * mouseXV);
    }

    private void FixedUpdate()
    {
        moveForce = cam.forward * moveX + cam.right * moveZ;
        rigid.AddForce(moveForce.normalized * moveSpeed);
    }
}