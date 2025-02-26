using UnityEngine;
using UnityEngine.UI;

public class SettingsTabSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Button generalButton;
    [SerializeField] private Button controlsButton;

    private void Start()
    {
        // Optionally default to General
        ShowGeneralPanel();

        generalButton.onClick.AddListener(ShowGeneralPanel);
        controlsButton.onClick.AddListener(ShowControlsPanel);
    }

    private void ShowGeneralPanel()
    {
        generalPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    private void ShowControlsPanel()
    {
        generalPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}