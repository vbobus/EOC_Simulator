using UnityEngine;
using UnityEngine.UI;

public class ScreenViewSwitcher : MonoBehaviour
{
    [SerializeField] private Button windowedButton;
    [SerializeField] private Button borderlessWindowButton;

    private void Start()
    {
        windowedButton.onClick.AddListener(SetWindowed);
        borderlessWindowButton.onClick.AddListener(SetBorderless);
    }

    private void SetWindowed()
    {
        Screen.fullScreen = false;
    }

    private void SetBorderless()
    {
        Screen.fullScreen = true;
    }
}