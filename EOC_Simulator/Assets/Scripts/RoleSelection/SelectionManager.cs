using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    [Header("Selections")]
    public RoleDefinition selectedRole;
    public ScenarioDefinition selectedScenario;

    [Header("Configuration")]
    public RoleScenarioConfig roleScenarioConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    public GameObject GetInteractivesPrefab()
    {
        if (roleScenarioConfig == null || selectedRole == null || selectedScenario == null)
        {
            Debug.LogError("SelectionManager is missing configuration or selections.");
            return null;
        }
        return roleScenarioConfig.GetInteractivesPrefab(selectedRole, selectedScenario);
    }
}