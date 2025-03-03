// Example script for CloseButton
using UnityEngine;
using UnityEngine.UI;

public class CloseSettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            settingsPanel.SetActive(false);
        });
    }
}