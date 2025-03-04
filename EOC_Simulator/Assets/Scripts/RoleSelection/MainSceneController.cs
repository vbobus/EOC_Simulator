using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    void Start()
    {
        if (SelectionManager.Instance == null) return;
        
        // Immediately instantiate the correct prefab based on the selected role and scenario.
        GameObject interactivesPrefab = SelectionManager.Instance.GetInteractivesPrefab();
        if (interactivesPrefab != null)
        {
            Instantiate(interactivesPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No matching interactives prefab found for the selected role/scenario.");
        }
    }
}