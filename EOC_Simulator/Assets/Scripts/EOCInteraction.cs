using UnityEngine;
using UnityEngine.UI;
using LLMUnity;
using TMPro; // Make sure this namespace is correctly referenced

public class EOCInteraction : MonoBehaviour
{
    // References to UI elements
    private TMP_InputField playerInputField;
    private TextMeshProUGUI npcOutputText;

    // Current NPC character the player is interacting with
    private LLMCharacter currentNPCCharacter;

    // Flag to track interaction state
    private bool isInteracting = false;

    void Start()
    {
        // Get UI elements from UIManager singleton
        if (UIManager.Instance != null)
        {
            playerInputField = UIManager.Instance.playerInputField;
            npcOutputText = UIManager.Instance.npcOutputText;
        }
        else
        {
            Debug.LogError("UIManager instance not found!");
            return;
        }

        // No need to enable or disable UI elements here; UIManager handles their state
    }

    void Update()
    {
        // Check for player input when interacting
        if (isInteracting && Input.GetKeyDown(KeyCode.Return))
        {
            string playerMessage = playerInputField.text;
            playerInputField.text = ""; // Clear the input field

            if (currentNPCCharacter != null && !string.IsNullOrWhiteSpace(playerMessage))
            {
                // Send the player's message to the NPC
                _ = currentNPCCharacter.Chat(playerMessage, HandleNPCReply, OnNPCReplyCompleted);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("character") && !isInteracting)
        {
            Debug.Log("Interacting");
            
            if (other.gameObject == this.gameObject) return; // Ignore self-collision

            // Start interaction
            isInteracting = true;

            // Get the LLMCharacter component from the NPC
            currentNPCCharacter = other.GetComponent<LLMCharacter>();
            if (currentNPCCharacter == null)
            {
                Debug.LogError("The NPC does not have an LLMCharacter component.");
                return;
            }

            // Show UI elements via UIManager
            UIManager.Instance.ShowInteractionUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("character") && isInteracting)
        {
            Debug.Log("NOt Interacting");
            
            if (other.gameObject == this.gameObject) return; // Ignore self-collision

            // End interaction
            isInteracting = false;
            currentNPCCharacter = null;

            // Hide UI elements via UIManager
            UIManager.Instance.HideInteractionUI();
        }
    }

    // Handle the NPC's reply (called continuously if streaming is enabled)
    void HandleNPCReply(string reply)
    {
        if (npcOutputText != null) 
            npcOutputText.text = reply;
    }

    // Called when the NPC has completed their reply
    void OnNPCReplyCompleted()
    {
        // Optional: Implement any post-reply logic here
    }
}