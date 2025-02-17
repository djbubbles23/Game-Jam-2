using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class DragNDrapPieceScript : MonoBehaviour
{
    public bool dragging;
    private Vector3 surfaceDistance;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Camera cam;
    private Vector3 mousePos;
    public bool letGo;
    private float letGoTime;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        if(dragging) {
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, 5,3)) {
                //Debug.Log("Raycast Worked");
                var mousePosition = cam.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y,hit.distance));
                transform.position = mousePosition;
            }
            else {
                var mousePosition = cam.ScreenToWorldPoint(new Vector3(mousePos.x,mousePos.y,1.2f));
                transform.position = mousePosition;
            }
        }
        else if (letGo && letGoTime > 20) {
            gameObject.transform.position = initialPos;
            gameObject.transform.rotation = initialRot;
            letGo = false;
        }
        else {
            letGoTime++;
        }
    }

    void OnMouseDown()
    {
        //Debug.Log("Click");
        initialPos = gameObject.transform.position;
        initialRot = gameObject.transform.rotation;
        dragging = true;
    }

    void OnMouseUp()
    {
        //Debug.Log("Release");
        // gameObject.transform.position = initialPos;
        // gameObject.transform.rotation = initialRot;
        dragging = false;
        letGo = true;
    }

}