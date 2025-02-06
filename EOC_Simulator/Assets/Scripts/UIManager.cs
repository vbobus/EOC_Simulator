using UnityEngine;
using UnityEngine.UI;
using TMPro;  

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_InputField playerInputField;
    public TextMeshProUGUI npcOutputText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optional: Uncomment if you want UIManager to persist between scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playerInputField != null)
        {
            playerInputField.gameObject.SetActive(false);
        }

        if (npcOutputText != null)
        {
            npcOutputText.gameObject.SetActive(false);
        }
    }

    public void ShowInteractionUI()
    {
        if (playerInputField != null)
        {
            playerInputField.gameObject.SetActive(true);
            playerInputField.ActivateInputField();
        }

        if (npcOutputText != null)
        {
            npcOutputText.gameObject.SetActive(true);
            npcOutputText.text = "";
        }
    }

    public void HideInteractionUI()
    {
        if (playerInputField != null)
        {
            playerInputField.gameObject.SetActive(false);
        }

        if (npcOutputText != null)
        {
            npcOutputText.gameObject.SetActive(false);
        }
    }
}