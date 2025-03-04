using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Scripts
{
    public class PageNavigationManager : MonoBehaviour
    {
        // UI Containers
        private VisualElement mainMenu;
        private VisualElement choosingCharacter;
        private VisualElement choosingScenario;
        private VisualElement introduction;
        private VisualElement phonePage;
        
        // Position information panels for roles
        private Dictionary<string, VisualElement> positionInformationPanels;

        // Navigation buttons
        private Button startButton;
        private Button backToMainMenuButton;
        private Button backToChoosingPage;
        private Button gameStart;
        private Button next;
        private Button beginSimulatorButton;
        private Button acceptButton;

        // Role (character) selection buttons
        private Dictionary<string, Button> characterButtons;

        // Scenario selection buttons (assumed to be part of the choosingScenario container)
        private Dictionary<string, Button> scenarioButtons;

        // Public lists for available roles and scenarios (assign via Inspector)
        public List<RoleDefinition> availableRoles;
        public List<ScenarioDefinition> availableScenarios;

        // Lookup dictionaries for fast asset lookup by key (roleName or scenarioName)
        private Dictionary<string, RoleDefinition> roleAssetLookup = new Dictionary<string, RoleDefinition>();
        private Dictionary<string, ScenarioDefinition> scenarioAssetLookup = new Dictionary<string, ScenarioDefinition>();

        void Start()
        {
    
    if (SelectionManager.Instance == null)
    {
        Debug.LogError("SelectionManager is not active! Make sure it is loaded and its Awake method has run.");
        // Optionally, you could instantiate one from a prefab here if desired.
    }
    
    var root = GetComponent<UIDocument>().rootVisualElement;
    if(root == null)
    {
        Debug.LogError("UIDocument rootVisualElement is null!");
        return;
    }

    // UI Containers
    mainMenu = root.Q<VisualElement>("MainMenu");
    choosingCharacter = root.Q<VisualElement>("ChoosingCharacter");
    choosingScenario = root.Q<VisualElement>("ChoosingScenario");
    introduction = root.Q<VisualElement>("Introduction");
    phonePage = root.Q<VisualElement>("PhonePage");

    // Ensure these containers are not null
    if(mainMenu == null) Debug.LogError("MainMenu container not found in UXML!");
    if(choosingCharacter == null) Debug.LogError("ChoosingCharacter container not found in UXML!");

    // Initialize Position Information Panels
    positionInformationPanels = new Dictionary<string, VisualElement>
    {
        { "EOC_Director", root.Q<VisualElement>("PositionInformation_EOCDirector") },
        { "Planning_Chief", root.Q<VisualElement>("PositionInformation_PlanningChief") },
        { "Operations_Chief", root.Q<VisualElement>("PositionInformation_OperationsChief") }
    };

    // Navigation buttons
    startButton = root.Q<Button>("StartButton");
    backToMainMenuButton = root.Q<Button>("BackToMainMenu");
    backToChoosingPage = root.Q<Button>("BackToCharacterPage");
    gameStart = root.Q<Button>("GameStart");
    next = root.Q<Button>("Next");
    beginSimulatorButton = root.Q<Button>("BeginSimulator");
    acceptButton = root.Q<Button>("Accept");

    // Role buttons
    characterButtons = new Dictionary<string, Button>
    {
        { "EOC_Director", root.Q<Button>("EOC_Director") },
        { "Planning_Chief", root.Q<Button>("Planning_Chief") },
        { "Operations_Chief", root.Q<Button>("Operations_Chief") }
    };

    // Verify role buttons are not null
    foreach (var kvp in characterButtons)
    {
        if(kvp.Value == null)
            Debug.LogError($"Button for key {kvp.Key} not found in UXML!");
    }

    // Scenario buttons
    scenarioButtons = new Dictionary<string, Button>
    {
        { "Typhoon", root.Q<Button>("Typhoon") },
        { "Earthquake", root.Q<Button>("Earthquake") },
        { "Spillage", root.Q<Button>("Spillage") }
    };

    // Build lookup dictionary for roles
    foreach (var role in availableRoles)
    {
        if (!roleAssetLookup.ContainsKey(role.roleName))
        {
            roleAssetLookup.Add(role.roleName, role);
        }
        else
        {
            Debug.LogWarning("Duplicate role name found: " + role.roleName);
        }
    }
    // Log available keys
    foreach(var key in roleAssetLookup.Keys)
    {
        Debug.Log("Role key in lookup: " + key);
    }

    // Build lookup dictionary for scenarios
    foreach (var scenario in availableScenarios)
    {
        if (!scenarioAssetLookup.ContainsKey(scenario.scenarioName))
        {
            scenarioAssetLookup.Add(scenario.scenarioName, scenario);
        }
        else
        {
            Debug.LogWarning("Duplicate scenario name found: " + scenario.scenarioName);
        }
    }

    // Bind navigation button events (as before)
    startButton.clicked += ShowChoosingCharacterPage;
    backToMainMenuButton.clicked += ShowMainMenu;
    backToChoosingPage.clicked += ShowChoosingCharacterPage;
    next.clicked += ShowChoosingScenario;
    gameStart.clicked += ShowIntroduction;
    beginSimulatorButton.clicked += ShowPhonePage;

    // Bind role button click events
    foreach (var kvp in characterButtons)
    {
        string roleKey = kvp.Key;
        Button roleButton = kvp.Value;
        roleButton.clicked += () =>
        {
            if (roleAssetLookup.TryGetValue(roleKey, out RoleDefinition roleAsset))
            {
                SelectionManager.Instance.selectedRole = roleAsset;
                Debug.Log($"Selected Role updated to: {roleAsset.roleName}");
            }
            else
            {
                Debug.LogWarning("No RoleDefinition found for key: " + roleKey);
            }

            // Visual highlight for role buttons
            foreach (var btn in characterButtons.Values)
            {
                btn.RemoveFromClassList("selected");
            }
            roleButton.AddToClassList("selected");

            ShowInformationOfPosition(roleKey);
        };
    }

    // Bind scenario button click events (similar binding)
    foreach (var kvp in scenarioButtons)
    {
        string scenarioKey = kvp.Key;
        Button scenarioButton = kvp.Value;
        scenarioButton.clicked += () =>
        {
            if (scenarioAssetLookup.TryGetValue(scenarioKey, out ScenarioDefinition scenarioAsset))
            {
                SelectionManager.Instance.selectedScenario = scenarioAsset;
                Debug.Log($"Selected Scenario updated to: {scenarioAsset.scenarioName}");
            }
            else
            {
                Debug.LogWarning("No ScenarioDefinition found for key: " + scenarioKey);
            }

            foreach (var btn in scenarioButtons.Values)
            {
                btn.RemoveFromClassList("selected");
            }
            scenarioButton.AddToClassList("selected");
        };
    }
}


        private void ShowChoosingCharacterPage()
        {
            mainMenu.style.display = DisplayStyle.None;
            choosingScenario.style.display = DisplayStyle.None;
            introduction.style.display = DisplayStyle.None;
            phonePage.style.display = DisplayStyle.None;
            choosingCharacter.style.display = DisplayStyle.Flex;

            // Hide all Position Information panels when entering character selection
            foreach (var panel in positionInformationPanels.Values)
            {
                panel.style.display = DisplayStyle.None;
            }
        }

        private void ShowMainMenu()
        {
            choosingCharacter.style.display = DisplayStyle.None;
            mainMenu.style.display = DisplayStyle.Flex;
            choosingScenario.style.display = DisplayStyle.None;
            introduction.style.display = DisplayStyle.None;
            phonePage.style.display = DisplayStyle.None;
        }

        private void ShowInformationOfPosition(string characterName)
        {
            // Hide all information panels
            foreach (var panel in positionInformationPanels.Values)
            {
                panel.style.display = DisplayStyle.None;
            }

            // Show the information panel corresponding to the selected role
            if (positionInformationPanels.ContainsKey(characterName))
            {
                positionInformationPanels[characterName].style.display = DisplayStyle.Flex;
            }
        }

        private void ShowChoosingScenario()
        {
            mainMenu.style.display = DisplayStyle.None;
            choosingCharacter.style.display = DisplayStyle.None;
            introduction.style.display = DisplayStyle.None;
            phonePage.style.display = DisplayStyle.None;
            choosingScenario.style.display = DisplayStyle.Flex;
        }

        private void ShowIntroduction()
        {
            mainMenu.style.display = DisplayStyle.None;
            choosingCharacter.style.display = DisplayStyle.None;
            choosingScenario.style.display = DisplayStyle.None;
            phonePage.style.display = DisplayStyle.None;
            introduction.style.display = DisplayStyle.Flex;
        }

        private void ShowPhonePage()
        {
            mainMenu.style.display = DisplayStyle.None;
            choosingCharacter.style.display = DisplayStyle.None;
            choosingScenario.style.display = DisplayStyle.None;
            phonePage.style.display = DisplayStyle.Flex;
            introduction.style.display = DisplayStyle.None;
        }
    }
}
