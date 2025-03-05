using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SignInSignOutForm : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField nameInputField;  // Name input field
    public TMP_InputField checkInTimeText; // Check-in time field
    public TMP_InputField checkOutTimeText; // Check-out time field
    public Button completeButton; // "Complete" button

    private bool isSigningOut = false;  // Flag to determine if signing out
    private string playerName = "";     // Stores the player's name
    private string checkInTime = "";    // Stores the check-in time

    void Start()
    {
        completeButton.onClick.AddListener(OnCompleteClicked);
        gameObject.SetActive(false); // Hide UI by default
    }

    void OnEnable()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            // **Check-in mode**
            isSigningOut = false;
            nameInputField.interactable = true; // Allow input
            nameInputField.text = "";
            nameInputField.image.color = Color.yellow; // Highlight the name input field
            checkInTimeText.text = DateTime.Now.ToString("hh:mm tt"); // Auto-fill check-in time
            checkOutTimeText.text = "Enter text..."; // Leave check-out time empty
        }
        else
        {
            // **Check-out mode**
            isSigningOut = true;
            nameInputField.interactable = false; // Disable name input
            nameInputField.image.color = Color.white; // Remove highlight
            checkOutTimeText.text = DateTime.Now.ToString("hh:mm tt"); // Auto-fill check-out time
        }
    }

    void OnCompleteClicked()
    {
        if (!isSigningOut)
        {
            // **Check-in completed**
            playerName = nameInputField.text; // Store player name
            checkInTime = checkInTimeText.text; // Store check-in time
        }
        else
        {
            // **Check-out completed**
            Debug.Log($"Player {playerName} checked in at {checkInTime} and checked out at {checkOutTimeText.text}");
        }

        gameObject.SetActive(false); // Close UI
    }

    public void OpenForm()
    {
        gameObject.SetActive(true);
    }
}