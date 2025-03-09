using System;
using UnityEngine;
using UnityEngine.UI;
using Character.Player;
using Events;
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

    private int _currentMovementIndex = 0;
    private int _maxMovementIndex = 0;
    private void Start()
    {
        leftArrowButton.onClick.AddListener(PrevOption);
        rightArrowButton.onClick.AddListener(NextOption);

        _currentMovementIndex = (int)PlayerController.MovementType;
        _maxMovementIndex = Enum.GetValues(typeof(InputMovementTypes)).Length;
        UpdateOptionDisplay();
    }

    private void PrevOption()
    {
        _currentMovementIndex = (_currentMovementIndex + 1) % _maxMovementIndex;
        UpdateOptionDisplay(); 
    }

    private void NextOption()
    {
        _currentMovementIndex = (_currentMovementIndex + 1) % _maxMovementIndex;
        UpdateOptionDisplay();
    }

    private void UpdateOptionDisplay()
    {
        if (_currentMovementIndex == 0)
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