using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSControllScript : MonoBehaviour
{
    [SerializeField] Transform cam;
    public float mouseSens = 2f;         //Mouse's sensitivity
    private float mouseXV;          //
    private float mouseYV;          //
    private float camRotX = 0f;      //Current Rotation of the camera in the X axis
    private float camRotY = 0f;      //Current Rotation of the camera in the Y axis


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Inputs
        mouseXV = Input.GetAxis("Mouse X") * mouseSens;
        mouseYV = Input.GetAxis("Mouse Y") * mouseSens;


        //Camera handeling
        camRotY -= mouseYV;
        camRotY = Math.Clamp(camRotY, -90f, 90f);

        cam.localEulerAngles = Vector3.right * camRotY;
        transform.Rotate(Vector3.up * mouseXV);


    }
}