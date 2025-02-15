using UnityEngine;

public class ItemMoveScript : MonoBehaviour
{
    private Vector3 initialPos; //Initial position
    private Quaternion initialRot; //initial rotation
    
    public Transform objPos; //Position of the object
    private Transform moveTo; //Location to move to
    void Start()
    {
        objPos = gameObject.transform;
        initialPos = objPos.position;
        initialRot = objPos.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveObj(Transform endPos) {
        gameObject.transform.position = endPos.position;
        gameObject.transform.rotation = endPos.rotation;
    }

    public void PutDown() {
        gameObject.transform.position = initialPos;
        gameObject.transform.rotation = initialRot;
    }
}
