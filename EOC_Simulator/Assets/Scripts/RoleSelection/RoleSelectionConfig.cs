using UnityEngine;

[System.Serializable]
public class RoleScenarioMapping
{
    public RoleDefinition role;
    public ScenarioDefinition scenario;
    public GameObject interactivesPrefab;
}

[CreateAssetMenu(fileName = "RoleScenarioConfig", menuName = "EOC/Role Scenario Config")]
public class RoleScenarioConfig : ScriptableObject
{
    public RoleScenarioMapping[] mappings;

    public GameObject GetInteractivesPrefab(RoleDefinition role, ScenarioDefinition scenario)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.role == role && mapping.scenario == scenario)
            {
                return mapping.interactivesPrefab;
            }
        }
        return null;
    }
}