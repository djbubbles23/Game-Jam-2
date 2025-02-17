using UnityEngine;

public class CheckChildren : MonoBehaviour
{
    public bool jigsawComplete = false;
    void Update()
    {
        bool allActive = AreAllChildrenActive();
        if(allActive){
            jigsawComplete = true;
            Debug.Log("Jigsaw Complete!");
        }
        Debug.Log("All children active: " + allActive);

    }

    bool AreAllChildrenActive()
    {
        DragAndMoveSprite3D[] childrenScripts = GetComponentsInChildren<DragAndMoveSprite3D>();

        foreach (DragAndMoveSprite3D child in childrenScripts)
        {
            if (!child.pieceStatus) // Assuming "isActive" is a public bool in MyChildScript
            {
                return false; // If one child is not active, return false
            }
        }

        return true; // All children passed the check
    }
}