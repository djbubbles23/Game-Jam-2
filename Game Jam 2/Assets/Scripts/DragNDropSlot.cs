using UnityEngine;

public class DragNDropSlot : MonoBehaviour
{
    [SerializeField] GameObject piece;
    [SerializeField] GameObject manager;

    void Update()
    {
        if (Vector3.Distance(piece.transform.position,gameObject.transform.position) < .05 
                && piece.GetComponent<DragNDrapPieceScript>().letGo) {
            Debug.Log("Yay");
            piece.SetActive(false);
            manager.GetComponent<DragNDropManager>().solvedCount++;
        }
    }
}
