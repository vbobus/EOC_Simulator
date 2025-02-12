using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI_Scripts
{
    public class PageNavigationManager : MonoBehaviour
    {
        private VisualElement mainMenu;
        private VisualElement choosingCharacter;
        private VisualElement choosingScenario;
        private VisualElement introduction;
        private VisualElement phonePage;
        private Dictionary<string, VisualElement> positionInformationPanels;

        private Button startButton;
        private Button backToMainMenuButton;
        private Dictionary<string, Button> characterButtons;
        private Button backToChoosingPage;
        private Button gameStart;
        private Button next;
        private Button beginSimulatorButton;
        private Button acceptButton;

        void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            // Get UI elements
            mainMenu = root.Q<VisualElement>("MainMenu");
            choosingCharacter = root.Q<VisualElement>("ChoosingCharacter");
            choosingScenario = root.Q<VisualElement>("ChoosingScenario");
            introduction = root.Q<VisualElement>("Introduction");
            phonePage = root.Q<VisualElement>("PhonePage");

            // Initialize Position Information Panels
            positionInformationPanels = new Dictionary<string, VisualElement>
            {
                { "EOC_Director", root.Q<VisualElement>("PositionInformation_EOCDirector") },
                { "Planning_Chief", root.Q<VisualElement>("PositionInformation_PlanningChief") },
                { "Operations_Chief", root.Q<VisualElement>("PositionInformation_OperationsChief") }
                // Additional role information panels can be added here
            };

            // Get buttons
            startButton = root.Q<Button>("StartButton");
            backToMainMenuButton = root.Q<Button>("BackToMainMenu");
            backToChoosingPage = root.Q<Button>("BackToCharacterPage");
            gameStart = root.Q<Button>("GameStart");
            next = root.Q<Button>("Next");
            beginSimulatorButton = root.Q<Button>("BeginSimulator");
            acceptButton = root.Q<Button>("Accept");

            characterButtons = new Dictionary<string, Button>
            {
                { "EOC_Director", root.Q<Button>("EOC_Director") },
                { "Planning_Chief", root.Q<Button>("Planning_Chief") },
                { "Operations_Chief", root.Q<Button>("Operations_Chief") }
            };

            // Bind events
            startButton.clicked += ShowChoosingCharacterPage;
            backToMainMenuButton.clicked += ShowMainMenu;
            backToChoosingPage.clicked += ShowChoosingCharacterPage;
            next.clicked += ShowChoosingScenario;
            gameStart.clicked += ShowIntroduction;
            beginSimulatorButton.clicked += ShowPhonePage;

            foreach (var character in characterButtons)
            {
                character.Value.clicked += () => ShowInformationOfPosition(character.Key);
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

            // Show the current character's information panel
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
