using UnityEngine;

public class DragDropScript : MonoBehaviour
{
    public GameObject objectToDrag;
    public GameObject objectDestination;
    public float dropDist;
    public bool locked;
    Vector2 objectInitPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectInitPos = objectToDrag.transform.position;
        Debug.Log("Drag Startup");
    }

    public void DragObject() {
        Debug.Log("Dragging 1");
        if (!locked) {
            Debug.Log("Dragging 2");
            objectToDrag.transform.position = Input.mousePosition;
        }
    }

    public void DropObject() {
        float distance = Vector3.Distance(objectToDrag.transform.position, objectDestination.transform.position);
        if (distance <= dropDist) {
            locked = true;
            objectToDrag.transform.position = objectDestination.transform.position;
        }
    }
}
