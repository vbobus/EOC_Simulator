using System;
using UnityEngine;

namespace Character.Player
{
    public enum InputMovementTypes
    {
        WASD_MOUSE_TO_ROTATE, // WASD to move and Mouse will be used to rotate 
        WASD_ONLY,            // Client idea of movement, WS => forward, back.   AD => rotate left or right
        MOUSE_ONLY            // Click to move and right click to rotate
    }
    
    public class PlayerController : MonoBehaviour
    {
        public static InputMovementTypes MovementType => InputMovementTypes.WASD_ONLY;
        [SerializeField] private LayerMask collisionLayerMask;
        [SerializeField] private float movementSpeed = 5.0f;
        [SerializeField] private float rotationSpeed = 0.8f;

        private CharacterController _characterController;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void HandleInput(Vector2 directionMovement, Vector2 delta, Vector2 lookPosition)
        {
            switch (MovementType)
            {
                case InputMovementTypes.WASD_MOUSE_TO_ROTATE:
                    HandleWasdMouseToRotate(directionMovement, delta);
                    break;
                case InputMovementTypes.WASD_ONLY:
                    HandleWasdOnly(directionMovement);
                    break;
                case InputMovementTypes.MOUSE_ONLY:
                    break;
            }
        }

        private void HandleWasdMouseToRotate(Vector2 directionMovement, Vector2 delta)
        {
            _characterController.Move(directionMovement * (movementSpeed * Time.deltaTime));
            
            transform.Rotate(0, delta.x * (rotationSpeed * Time.deltaTime), 0);
        }

        private void HandleWasdOnly(Vector2 directionMovement)
        {
            // Movement direction based on transform.forward
            Vector3 movement = transform.forward * directionMovement.y;
            movement = movement.normalized;

            if (directionMovement.y < 0)
                movement *= 0.5f; // Move by half speed, when walking back

            //
            _characterController.Move(movement * (movementSpeed * Time.deltaTime));
            
            // Rotate based on A/D keys
            transform.Rotate(0, directionMovement.x * (rotationSpeed * Time.deltaTime), 0);
        }

    }
}