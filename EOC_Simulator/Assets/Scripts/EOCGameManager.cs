using LLMUnity;
using UnityEngine;
using UnityEngine.UI;

public class EOCGameManager : MonoBehaviour
{
    public GameObject[] characterPrefabs; 
    public Transform playerSpawnPoint;
    public Transform[] npcSpawnPoints;

    private string playerRole;
    private int npcSpawnIndex = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("SelectedRole"))
        {
            playerRole = PlayerPrefs.GetString("SelectedRole");
        }
        else
        {
            Debug.LogError("No role selected! Defaulting to EOC Director.");
            playerRole = "EOC Director"; 
        }
        
        InstantiateCharacters();
    }

    void InstantiateCharacters()
    {
        GameObject playerCharacter = null;

        foreach (GameObject prefab in characterPrefabs)
        {
            CharacterConfig config = prefab.GetComponent<CharacterConfig>();

            if (config.characterRole == playerRole)
            {
                playerCharacter = Instantiate(prefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                
                Character.Player.PlayerController playerCtrl = playerCharacter.GetComponent<Character.Player.PlayerController>();
                if (playerCtrl != null) playerCtrl.enabled = true;
                
                NPCController npcCtrl = playerCharacter.GetComponent<NPCController>();
                if (npcCtrl != null) npcCtrl.enabled = false;
                
                EOCInteraction interaction = playerCharacter.GetComponent<EOCInteraction>();
                if (interaction == null)
                {
                    interaction = playerCharacter.AddComponent<EOCInteraction>();
                }
                interaction.enabled = true;
                
                LLMCharacter llmChar = playerCharacter.GetComponent<LLMCharacter>();
                if (llmChar != null)
                {
                    llmChar.llm = GameObject.Find("LLMManager").GetComponent<LLM>();
                }
                
            }
            else
            {
                Transform spawnPoint = GetNextNPCSpawnPoint();
                GameObject npcCharacter = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                
                NPCController npcCtrl = npcCharacter.GetComponent<NPCController>();
                if (npcCtrl != null) npcCtrl.enabled = !npcCtrl.enabled ? true : npcCtrl.enabled; 
                
                PlayerController playerCtrl = npcCharacter.GetComponent<PlayerController>();
                if (playerCtrl != null) playerCtrl.enabled = false;
                
                EOCInteraction interaction = npcCharacter.GetComponent<EOCInteraction>();
                if (interaction != null) interaction.enabled = false;
                
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