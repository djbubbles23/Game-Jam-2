using UnityEngine;
using System.Collections.Generic;

public enum ModelStates
{
    Idle,
    Walking
}

[System.Serializable]
public class DollModelEntry
{
    public ModelStates modelName;
    public GameObject modelObject;
    [Tooltip("Local offset to apply to the model when activated. Adjust Y to lower the model if it floats.")]
    public Vector3 modelOffset;
}

public class DollModelSwitcher : MonoBehaviour
{
    [Tooltip("List of models to choose from. They should be children of this GameObject.")]
    public List<DollModelEntry> dollModels = new List<DollModelEntry>();

    private ModelStates currentModel = ModelStates.Idle;

    /// <summary>
    /// Switches the active model.
    /// </summary>
    /// <param name="modelName">The model state to switch to.</param>
    public void SwitchToModel(ModelStates modelName)
    {
        // If already on this model, do nothing.
        if (currentModel == modelName)
            return;

        foreach (var entry in dollModels)
        {
            if (entry.modelObject != null)
            {
                bool shouldActivate = (entry.modelName == modelName);
                entry.modelObject.SetActive(shouldActivate);
                if (shouldActivate)
                {
                    // Apply the offset so the model is positioned correctly.
                    entry.modelObject.transform.localPosition = entry.modelOffset;
                    currentModel = modelName;
                }
            }
        }
    }
}