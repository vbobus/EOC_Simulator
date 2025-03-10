using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LaptopUIController : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject laptopScreenPanel;  

    [Header("Panels")]
    [SerializeField] private GameObject panelSelectDisplays; 
    [SerializeField] private GameObject panelResults;        

    [Header("Selection Panel UI")]
    [SerializeField] private Button[] displayButtons;       
    [SerializeField] private TextMeshProUGUI selectionCountText;
    [SerializeField] private Button confirmButton;

    [Header("Results Panel UI")]
    [SerializeField] private TextMeshProUGUI resultsTitleText;
    [SerializeField] private Button updateDisplaysButton;  
    [SerializeField] private Button closeButton;

    [Header("Minigame Settings")]
    [SerializeField] private int maxSelections = 3; 
    [SerializeField] private string[] correctAnswers = {
        "Situation/Event Map",
        "Weather Forecast Board",
        "Issues and Action Tracker"
    };

    [Header("Selection Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    // Tracking button selections
    private bool[] isSelected;
    private int correctCount = 0;

    private void Awake()
    {
        // Prepare the isSelected array to match the number of display buttons
        isSelected = new bool[displayButtons.Length];
        
        for (int i = 0; i < displayButtons.Length; i++)
        {
            int index = i; 
            displayButtons[i].onClick.AddListener(() => OnDisplayButtonClicked(index));
        }

        // Hook up Confirm, Update, and Close
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmSelection);
        if (updateDisplaysButton != null) updateDisplaysButton.onClick.AddListener(OnUpdateDisplaysClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseLaptop);
    }

    private void Start()
    {
        // Hide the entire laptop UI at startup
        if (laptopScreenPanel != null) laptopScreenPanel.SetActive(false);

        // Optionally ensure panels are in their initial states
        if (panelResults != null) panelResults.SetActive(false);
        if (panelSelectDisplays != null) panelSelectDisplays.SetActive(true);

        // Initialize button colors and selection count text
        for (int i = 0; i < displayButtons.Length; i++)
        {
            SetButtonColor(i, normalColor);
        }
        UpdateSelectionsCount();
    }

    /// <summary>
    /// Public method to show the laptop UI.
    /// Call this from a Dialogue System trigger (SendMessage or UnityEvent).
    /// </summary>
    public void ShowLaptopUI()
    {
        if (laptopScreenPanel != null)
        {
            laptopScreenPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Called when a display button is clicked.
    /// Toggles selection state and enforces max selection limit.
    /// </summary>
    private void OnDisplayButtonClicked(int index)
    {
        bool currentlySelected = isSelected[index];

        if (currentlySelected)
        {
            // Unselect
            isSelected[index] = false;
            SetButtonColor(index, normalColor);
        }
        else
        {
            // If not selected, check limit
            if (CountSelected() < maxSelections)
            {
                isSelected[index] = true;
                SetButtonColor(index, selectedColor);
            }
            else
            {
                Debug.Log("Cannot select more than " + maxSelections);
            }
        }

        UpdateSelectionsCount();
    }

    /// <summary>
    /// Updates the "Selected X/3" text.
    /// </summary>
    private void UpdateSelectionsCount()
    {
        selectionCountText.text = $"Selected {CountSelected()}/{maxSelections} displays";
    }

    private int CountSelected()
    {
        return isSelected.Count(s => s);
    }

    private void SetButtonColor(int index, Color color)
    {
        var buttonImage = displayButtons[index].GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    /// <summary>
    /// Called when ConfirmButton is clicked.
    /// </summary>
    private void OnConfirmSelection()
    {
        // Determine how many are correct
        correctCount = 0;

        for (int i = 0; i < displayButtons.Length; i++)
        {
            if (isSelected[i])
            {
                string buttonName = displayButtons[i].gameObject.name;
                if (correctAnswers.Contains(buttonName))
                {
                    correctCount++;
                }
            }
        }

        // Show results
        if (resultsTitleText != null)
        {
            resultsTitleText.text = $"You got {correctCount}/{maxSelections} correct!";
        }

        if (panelSelectDisplays != null) panelSelectDisplays.SetActive(false);
        if (panelResults != null) panelResults.SetActive(true);
    }

    /// <summary>
    /// Called when "UpdateDisplaysButton" is clicked (if you want to update external monitors, etc.)
    /// </summary>
    private void OnUpdateDisplaysClicked()
    {
        Debug.Log("UpdateDisplaysButton clicked. Implement update EOC monitors).");
    }

    /// <summary>
    /// Closes the laptop UI entirely.
    /// </summary>
    private void OnCloseLaptop()
    {
        if (laptopScreenPanel != null)
        {
            laptopScreenPanel.SetActive(false);
        }
    }
}
