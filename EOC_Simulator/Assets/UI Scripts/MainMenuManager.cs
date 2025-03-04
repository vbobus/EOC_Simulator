using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI_Scripts
{
    public class MainMenuManager : MonoBehaviour
    {
        private Button startButton;
        [SerializeField] private string nextLoadSceneName = "Main";
        void Start()
        {
            // Get the root VisualElement
            var root = GetComponent<UIDocument>().rootVisualElement;

            // Find the button and bind the click event
            startButton = root.Q<Button>("Accept");
            startButton.clicked += StartGame;
        }

        private void StartGame()
        {
            // Switch to the main game scene
            SceneManager.LoadScene(nextLoadSceneName);
        }
    }
}