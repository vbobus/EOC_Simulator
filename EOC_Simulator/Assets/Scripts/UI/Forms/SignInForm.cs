using System;
using Events;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Forms
{
    public class SignInForm : MonoBehaviour
    {
        // [SerializeField] private DialogueSystemTrigger seleteced
        [SerializeField] private GameObject signInFormsPanel;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_Dropdown timeInDropdown;
        [SerializeField] private TMP_Dropdown timeOutDropdown;

        [SerializeField] private Button completeButton;
        
        // Using a enum, to make code more readable that a bool that cant be changed
        private enum SignState {SignIn, SignOut}
        private SignState _signState;

        private void Awake()
        {
            signInFormsPanel.SetActive(false);
        }

        private void Start()
        {
            completeButton.onClick.AddListener(OnCompleteClicked);
        }

        
        // Make a popup that will display for some time the error.
        private void OnCompleteClicked()
        {
            if (_signState.Equals(SignState.SignIn))
            {
                if (String.IsNullOrEmpty(nameInputField.text)) // Name not set
                {
                    Debug.Log($"Name not set");
                    return;
                }

                if (timeInDropdown.value == 0)
                {
                    Debug.Log($"Time In not set");
                    return;
                }
            }
            else
            {
                if (timeOutDropdown.value == 0)
                {
                    Debug.Log($"Time Out not set");
                    return;                    
                }
            }
            
            signInFormsPanel.SetActive(false);
            InputManager.Instance.SwitchToPlayerMapInDialogue();
        }

        public void OnStartSignIn()
        {
            InputManager.Instance.SwitchToUIMapInDialogue();
            _signState = SignState.SignIn;

            nameInputField.enabled = true;
            timeInDropdown.enabled = true;
            timeOutDropdown.enabled = false;
            
            signInFormsPanel.SetActive(true);
        }
        
        public void OnStartSignOut()
        {
            InputManager.Instance.SwitchToUIMapInDialogue();
            _signState = SignState.SignOut;
            
            nameInputField.enabled = false;
            timeInDropdown.enabled = false;
            timeOutDropdown.enabled = true;
            
            signInFormsPanel.SetActive(true);
        }
    }
}
