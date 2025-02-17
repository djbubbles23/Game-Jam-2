using UnityEngine;

public class DragAndMoveSprite3D : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;

    public bool pieceStatus = false;
    

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        // Convert world position to screen point and get offset
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if(pieceStatus != true){
            // Update position based on mouse movement
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        // Get mouse position in screen space
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z; // Maintain depth

        // Convert screen space to world space
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == gameObject.name){
            transform.position = collision.gameObject.transform.position;
            pieceStatus = true;
        }
    }
}
