using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LaptopUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject panelSelectDisplays;
    [SerializeField] private GameObject panelResults;

    [Header("Selection Panel UI")]
    [SerializeField] private Button[] displayButtons;      // 6 Buttons in DisplayGrid
    [SerializeField] private TextMeshProUGUI selectionCountText;
    [SerializeField] private Button confirmButton;

    [Header("Results Panel UI")]
    [SerializeField] private TextMeshProUGUI resultsTitleText;
    [SerializeField] private Button updateDisplaysButton;  // triggers EOC monitor updates
    [SerializeField] private Button closeButton;
    // 'Answers' image is purely static, so we don't need a reference if it's not changing.

    [Header("Minigame Settings")]
    [SerializeField] private int maxSelections = 3; 
    [Tooltip("Names of the Buttons that are correct answers")]
    [SerializeField] private string[] correctAnswers = {
        "Situation/Event Map",
        "Weather Forecast Board",
        "Issues and Action Tracker"
    };

    [Header("EOC Monitors")]
    [SerializeField] private Image eocMonitor1;
    [SerializeField] private Image eocMonitor2;

    [Header("EOC Monitor Sprites")]
    [SerializeField] private Sprite allCorrectSprite;
    [SerializeField] private Sprite partialCorrectSprite;
    [SerializeField] private Sprite noCorrectSprite;

    // Track which buttons are selected
    private bool[] isSelected;  
    // Store how many were correct
    private int correctCount = 0;

    // Colors for selected vs. unselected buttons (example)
    [Header("Selection Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    private void Awake()
    {
        // Initialize the array to match the number of display buttons
        isSelected = new bool[displayButtons.Length];

        // Make sure the results panel is hidden at the start
        if (panelResults != null) panelResults.SetActive(false);

        // Set up button click listeners
        for (int i = 0; i < displayButtons.Length; i++)
        {
            int index = i;  // capture i in a local variable for the lambda
            displayButtons[i].onClick.AddListener(() => OnDisplayButtonClicked(index));
        }

        // Confirm, Update, Close button listeners
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmSelection);

        if (updateDisplaysButton != null)
            updateDisplaysButton.onClick.AddListener(OnUpdateDisplaysClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseResults);
    }

    private void Start()
    {
        UpdateSelectionsCount();
        // Ensure buttons have the correct initial color (unselected)
        for (int i = 0; i < displayButtons.Length; i++)
        {
            SetButtonColor(i, normalColor);
        }
    }

    /// <summary>
    /// Called when a display button is clicked.
    /// Toggles the selected state for that button index.
    /// </summary>
    private void OnDisplayButtonClicked(int index)
    {
        bool currentlySelected = isSelected[index];

        if (currentlySelected)
        {
            // If already selected, unselect
            isSelected[index] = false;
            SetButtonColor(index, normalColor);
        }
        else
        {
            // If not selected, check if we can select it
            if (CountSelected() < maxSelections)
            {
                isSelected[index] = true;
                SetButtonColor(index, selectedColor);
            }
            else
            {
                // Optionally give feedback: beep, flash, or do nothing
                Debug.Log("Cannot select more than " + maxSelections);
            }
        }

        UpdateSelectionsCount();
    }

    /// <summary>
    /// Updates the "Selected X/3 displays" text.
    /// </summary>
    private void UpdateSelectionsCount()
    {
        int selectedCount = CountSelected();
        selectionCountText.text = $"Selected {selectedCount}/{maxSelections} displays";
    }

    /// <summary>
    /// Returns how many buttons are currently selected.
    /// </summary>
    private int CountSelected()
    {
        return isSelected.Count(s => s);
    }

    /// <summary>
    /// Changes the button's image color to indicate selection or not.
    /// </summary>
    private void SetButtonColor(int index, Color color)
    {
        Image btnImage = displayButtons[index].GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.color = color;
        }
    }

    /// <summary>
    /// Called when user clicks the ConfirmButton.
    /// Counts how many selected are correct, shows results panel.
    /// </summary>
    private void OnConfirmSelection()
    {
        // 1. Gather the names of the selected buttons
        List<string> selectedNames = new List<string>();
        for (int i = 0; i < displayButtons.Length; i++)
        {
            if (isSelected[i])
            {
                // Use the button's GameObject name to compare with correctAnswers
                selectedNames.Add(displayButtons[i].gameObject.name);
            }
        }

        // 2. Determine how many are correct
        correctCount = 0;
        foreach (var name in selectedNames)
        {
            if (correctAnswers.Contains(name))
            {
                correctCount++;
            }
        }

        // 3. Update the results panel text
        if (resultsTitleText != null)
        {
            resultsTitleText.text = $"You got {correctCount}/{maxSelections} correct!";
        }

        // 4. Switch panels
        panelSelectDisplays.SetActive(false);
        panelResults.SetActive(true);
    }

    /// <summary>
    /// Called when the user clicks "UpdateDisplaysButton" on the results panel.
    /// Updates the EOC monitors in the scene with certain sprites.
    /// </summary>
    private void OnUpdateDisplaysClicked()
    {
        if (correctCount == maxSelections)
        {
            // All correct
            if (eocMonitor1 != null) eocMonitor1.sprite = allCorrectSprite;
            if (eocMonitor2 != null) eocMonitor2.sprite = allCorrectSprite;
        }
        else if (correctCount > 0)
        {
            // Some correct
            if (eocMonitor1 != null) eocMonitor1.sprite = partialCorrectSprite;
            if (eocMonitor2 != null) eocMonitor2.sprite = partialCorrectSprite;
        }
        else
        {
            // None correct
            if (eocMonitor1 != null) eocMonitor1.sprite = noCorrectSprite;
            if (eocMonitor2 != null) eocMonitor2.sprite = noCorrectSprite;
        }

        // (Optional) If using Dialogue System or quest logic:
        // PixelCrushers.DialogueSystem.Lua.Run("QuestLog.SetQuestState('UpdateDisplays','success')");
    }

    /// <summary>
    /// Closes the laptop UI.
    /// </summary>
    private void OnCloseResults()
    {
        // Hide the entire laptop screen panel
        this.gameObject.SetActive(false);
    }
}
