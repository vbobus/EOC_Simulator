using System;
using TMPro;
using UnityEngine;

public class TestShowMovementType : MonoBehaviour
{
    public TMP_Text text;
    public Character.Player.PlayerController playerController;

    private void Update()
    {
        text.text = $"MovementType: {playerController.movementType}";
    }
}
