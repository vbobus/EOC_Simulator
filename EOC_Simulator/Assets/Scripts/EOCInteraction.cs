using Events;
using UnityEngine;
using UnityEngine.UI;
using LLMUnity;
using TMPro; 

public class EOCInteraction : MonoBehaviour
{
    private TMP_InputField playerInputField;
    private TextMeshProUGUI npcOutputText;
    
    private LLMCharacter currentNPCCharacter;
    
    private bool isInteracting = false;

    void Start()
    {
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
    }

    void Update()
    {
        if (isInteracting && Input.GetKeyDown(KeyCode.Return))
        {
            string playerMessage = playerInputField.text;
            playerInputField.text = ""; 

            if (currentNPCCharacter != null && !string.IsNullOrWhiteSpace(playerMessage))
            {
                _ = currentNPCCharacter.Chat(playerMessage, HandleNPCReply, OnNPCReplyCompleted);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteracting)
        {
            Debug.Log("Interacting");
            
            //InputManager.Instance.SwitchToUIMap();
            
            if (other.gameObject == this.gameObject) return; 
            isInteracting = true;
            
            currentNPCCharacter = other.GetComponent<LLMCharacter>();
            if (currentNPCCharacter == null)
            {
                Debug.LogError("The NPC does not have an LLMCharacter component.");
                return;
            }
            
            UIManager.Instance.ShowInteractionUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInteracting)
        {
            Debug.Log("NOt Interacting");
            
            if (other.gameObject == this.gameObject) return; 
            isInteracting = false;
            currentNPCCharacter = null;
            
            UIManager.Instance.HideInteractionUI();
        }
    }
    
    void HandleNPCReply(string reply)
    {
        if (npcOutputText != null) 
            npcOutputText.text = reply;
    }
    
    void OnNPCReplyCompleted()
    {
        // Implement any post-reply logic here
    }
}