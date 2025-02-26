using UnityEngine;
using UnityEngine.UI;
using Character.Player;
using TMPro;

public class MovementControlSwitcher : MonoBehaviour
{
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Image optionsImage;
    [SerializeField] private TextMeshProUGUI optionsLabel;

    [SerializeField] private Sprite wasdSprite;
    [SerializeField] private Sprite mouseSprite;
    [SerializeField] private string wasdText = "Option 1: Keyboard [WASD]";
    [SerializeField] private string mouseText = "Option 2: Mouse [Point & Click]";

    [SerializeField] private PlayerController playerController;

    private int currentIndex = 0; // 0 => WASD, 1 => Mouse

    private void Start()
    {
        leftArrowButton.onClick.AddListener(PrevOption);
        rightArrowButton.onClick.AddListener(NextOption);

        UpdateOptionDisplay();
    }

    private void PrevOption()
    {
        currentIndex = (currentIndex + 1) % 2;
        UpdateOptionDisplay();
    }

    private void NextOption()
    {
        currentIndex = (currentIndex + 1) % 2;
        UpdateOptionDisplay();
    }

    private void UpdateOptionDisplay()
    {
        if (currentIndex == 0)
        {
            optionsImage.sprite = wasdSprite;
            optionsLabel.text = wasdText;
            playerController.ChangeMovementType(InputMovementTypes.WASD_MOUSE_TO_ROTATE);
        }
        else
        {
            optionsImage.sprite = mouseSprite;
            optionsLabel.text = mouseText;
            playerController.ChangeMovementType(InputMovementTypes.MOUSE_ONLY);
        }
    }
}