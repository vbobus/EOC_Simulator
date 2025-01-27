using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class EOCGameManager : MonoBehaviour
{
    public GameObject[] characterPrefabs; // Assign all character prefabs in the Inspector
    public Transform playerSpawnPoint;
    public Transform[] npcSpawnPoints;

    private string playerRole;
    private int npcSpawnIndex = 0;

    void Start()
    {
        // Retrieve the player's selected role
        if (PlayerPrefs.HasKey("SelectedRole"))
        {
            playerRole = PlayerPrefs.GetString("SelectedRole");
        }
        else
        {
            Debug.LogError("No role selected! Defaulting to EOC Director.");
            playerRole = "EOC Director"; // Default role
        }

        // Instantiate the player and NPCs
        InstantiateCharacters();
    }

    void InstantiateCharacters()
    {
        GameObject playerCharacter = null;

        foreach (GameObject prefab in characterPrefabs)
        {
            // Get the role from CharacterConfig
            CharacterConfig config = prefab.GetComponent<CharacterConfig>();

            if (config.characterRole == playerRole)
            {
                // Instantiate player character
                playerCharacter = Instantiate(prefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

                // Enable PlayerController
                PlayerController playerCtrl = playerCharacter.GetComponent<PlayerController>();
                if (playerCtrl != null) playerCtrl.enabled = true;

                // Disable NPCController
                NPCController npcCtrl = playerCharacter.GetComponent<NPCController>();
                if (npcCtrl != null) npcCtrl.enabled = false;

                // Add and enable EOCInteraction script
                EOCInteraction interaction = playerCharacter.GetComponent<EOCInteraction>();
                if (interaction == null)
                {
                    interaction = playerCharacter.AddComponent<EOCInteraction>();
                }
                interaction.enabled = true;

                // Assign LLM GameObject to LLMCharacter
                LLMCharacter llmChar = playerCharacter.GetComponent<LLMCharacter>();
                if (llmChar != null)
                {
                    // Assign the LLM GameObject (assuming it's named "LLMManager")
                    llmChar.llm = GameObject.Find("LLMManager").GetComponent<LLM>();
                }
            }
            else
            {
                // Instantiate NPC character
                Transform spawnPoint = GetNextNPCSpawnPoint();
                GameObject npcCharacter = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

                // Enable NPCController
                NPCController npcCtrl = npcCharacter.GetComponent<NPCController>();
                if (npcCtrl != null) npcCtrl.enabled = !npcCtrl.enabled ? true : npcCtrl.enabled; // Enabled if present

                // Disable PlayerController
                PlayerController playerCtrl = npcCharacter.GetComponent<PlayerController>();
                if (playerCtrl != null) playerCtrl.enabled = false;

                // Ensure EOCInteraction is disabled
                EOCInteraction interaction = npcCharacter.GetComponent<EOCInteraction>();
                if (interaction != null) interaction.enabled = false;

                // Assign LLM GameObject to LLMCharacter
                LLMCharacter llmChar = npcCharacter.GetComponent<LLMCharacter>();
                if (llmChar != null)
                {
                    llmChar.llm = GameObject.Find("LLMManager").GetComponent<LLM>();
                }
            }
        }
    }

    Transform GetNextNPCSpawnPoint()
    {
        if (npcSpawnPoints.Length == 0)
        {
            Debug.LogError("No NPC spawn points assigned.");
            return null;
        }
        Transform spawnPoint = npcSpawnPoints[npcSpawnIndex % npcSpawnPoints.Length];
        npcSpawnIndex++;
        return spawnPoint;
    }
}