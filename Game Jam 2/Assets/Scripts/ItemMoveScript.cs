using Unity.VisualScripting;
using UnityEngine;

public class ItemMoveScript : MonoBehaviour
{
    private Vector3 initialPos; //Initial position
    private Quaternion initialRot; //initial rotation
    public Transform objPos; //Position of the object
    private Transform moveTo; //Location to move to
    private Vector3 moveDistPos; //Distance to travel (position) to reach moveTo
    private Quaternion moveDistRot; //Distance to travel (rotation) to reach moveTo
    //[SerializeField] float moveSpeed = 2f; //Speed that the object moves from location to location

    void Start()
    {
        objPos = gameObject.transform;
        initialPos = objPos.position;
        initialRot = objPos.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveTo != null && objPos.position != moveTo.position) {
            objPos.position += moveDistPos / 20;
            moveDistPos -= moveDistPos/20;
        }
        
    }

    public void MoveObj(Transform endPos) {
        moveTo = endPos;
        moveDistPos = endPos.position - objPos.position;
    }

    public void PutDown() {
        moveTo.position = initialPos;
        moveDistPos = moveTo.position - objPos.position;
    }
}
