using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LLMUnity;
using System.Reflection;

[System.Serializable]
public class QuestLLMResponse 
{
    public string action;   // e.g., "StartQuest", "AdvanceQuest", "CompleteQuest", or "None"
    public string dialogue; // Dialogue to display on the output box
}

public class QuestStepValidator : MonoBehaviour
{
    [Header("LLM and UI References")]
    public LLMCharacter llmCharacter;      // Assign your LLMCharacter (from LLM for Unity) in the Inspector
    public TMP_InputField playerInputField; // The player's input field
    public TextMeshProUGUI npcOutputText;   // The NPC's dialogue output text

    [Header("Quest Step Settings")]
    [Tooltip("The current active quest ID. Must match an ActivityInfoSo.ID in the ActivityManager.")]
    public string currentQuestID = "QuestID123"; // Set dynamically or via the inspector
    [TextArea]
    [Tooltip("The validator prompt describing what the player's input must satisfy for this quest step.")]
    public string validatorPrompt = "Evaluate if the player's input meets the conditions to proceed with the quest.";

    void Start()
    {
        // Listen for player's submission
        playerInputField.onSubmit.AddListener(OnInputSubmit);
    }

    /// <summary>
    /// Called when the player submits input.
    /// </summary>
    async void OnInputSubmit(string message)
    {
        // Disable input while processing
        playerInputField.interactable = false;
        
        // Build the prompt that includes the validator prompt and the player's input.
        string prompt = ConstructValidatorPrompt(message);
        
        // Send the prompt to the LLM.
        string llmResponse = await llmCharacter.Chat(prompt);
        
        // Attempt to parse the response as JSON.
        QuestLLMResponse response = null;
        try
        {
            response = JsonUtility.FromJson<QuestLLMResponse>(llmResponse);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to parse LLM response: " + ex.Message);
        }
        
        if (response != null)
        {
            // If an action is specified (other than "None"), call the quest function.
            if (!string.IsNullOrEmpty(response.action) && response.action != "None")
            {
                string functionResult = CallQuestFunction(response.action, currentQuestID);
                // Append the function result to the dialogue feedback.
                response.dialogue += "\n" + functionResult;
            }
            
            // Display the dialogue.
            npcOutputText.text = response.dialogue;
        }
        else
        {
            npcOutputText.text = "Invalid response from LLM.";
        }
        
        // Re-enable player input.
        playerInputField.interactable = true;
    }

    /// <summary>
    /// Constructs a prompt that instructs the LLM to evaluate the player's input and return a JSON object.
    /// </summary>
    /// <param name="message">The player's input.</param>
    /// <returns>The full prompt to send to the LLM.</returns>
    string ConstructValidatorPrompt(string message)
    {
        string prompt = "You are an EOC NPC responsible for validating a quest step.\n";
        prompt += "The current quest ID is \"" + currentQuestID + "\".\n";
        prompt += "Quest Step Validator Instructions: " + validatorPrompt + "\n";
        prompt += "Player Input: \"" + message + "\"\n";
        prompt += "Based on the above, decide if the player's input satisfies the quest condition.\n";
        prompt += "Return a JSON object with two properties:\n";
        prompt += "  - \"action\": one of \"StartQuest\", \"AdvanceQuest\", \"CompleteQuest\", or \"None\" if no quest action should be taken.\n";
        prompt += "  - \"dialogue\": the text to display to the player as feedback.\n";
        prompt += "For example: {\"action\": \"StartQuest\", \"dialogue\": \"Excellent! Your input is valid and the quest has been started.\"}\n";
        prompt += "Output only the JSON object.";
        return prompt;
    }

    /// <summary>
    /// Uses reflection to call the corresponding method in QuestFunctions.
    /// </summary>
    /// <param name="functionName">The function name returned by the LLM.</param>
    /// <param name="questID">The current quest ID.</param>
    /// <returns>The result string returned from the quest function.</returns>
    string CallQuestFunction(string functionName, string questID)
    {
        MethodInfo method = typeof(QuestFunctions).GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);
        if (method != null)
        {
            object returnValue = method.Invoke(null, new object[] { questID });
            return returnValue != null ? returnValue.ToString() : "";
        }
        return "Quest function not found.";
    }
}
