using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using NUnit.Framework;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

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
        public static InputMovementTypes MovementType;
        
        [SerializeField] private FirstPersonCamera firstPersonCamera;
        [SerializeField] private LayerMask collisionLayerMask;
        [SerializeField] private float movementSpeed = 5.0f;
        
        private readonly float _walkBackMovementPenalty = 0.5f;
        
        [Title("MovementType", "WASD Only")]
        [SerializeField] private float rotationSpeedForWasdOnly = 0.8f;

        private CharacterController _characterController;
        private Vector2 _directionMovement;
        private Vector2 _delta;

        [Title("MovementType", "Pathfinding")]
        [SerializeField] private LayerMask aiWalkTargetLayerMask;
        [SerializeField] private Transform aiTargetMoveTowards;
        private IAstarAI _astarAI;

        [SerializeField] private Animator animator;
        
        #region SetUp
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _astarAI = GetComponent<IAstarAI>();
            
            // Subscribe to input events
            InputManager.Instance.On4DirectionMoveActionPressed += HandleDirectionMovement;
            InputManager.Instance.OnPointerDelta += HandlePointerDelta;

            InputManager.Instance.OnLeftClickActionPressed += HandleLeftClickPathfinding;
            InputManager.Instance.OnInteractActionPressed += TestChangeMovement;
         
            ChangeMovementType(InputMovementTypes.WASD_MOUSE_TO_ROTATE);
        }

        private void TestChangeMovement()
        {
            int current = (int)MovementType + 1;
            if (current >= Enum.GetValues(typeof(InputMovementTypes)).Length)
                current = 0;
            ChangeMovementType((InputMovementTypes)current);
        }

        public void ChangeMovementType(InputMovementTypes newMovementType)
        {
            MovementType = newMovementType;
            // Reset Camera
            firstPersonCamera.ResetCamera();
            
            // To stop the target from following around if we haven't selected Mouse Only input type 
            if (_astarAI == null) return;
            
            bool isMouseOnly = newMovementType == InputMovementTypes.MOUSE_ONLY;
            
            _astarAI.isStopped = !isMouseOnly;
            _astarAI.destination = transform.position;
            
            AIPath aiPath = _astarAI as AIPath;
            if (aiPath != null) aiPath.enabled = isMouseOnly;
                
            aiTargetMoveTowards.gameObject.SetActive(isMouseOnly);
        }

        private void OnDestroy()
        {
            // Unsubscribe from input events
            if (InputManager.Instance == null) return;
            InputManager.Instance.On4DirectionMoveActionPressed -= HandleDirectionMovement;
            InputManager.Instance.OnPointerDelta -= HandlePointerDelta;
        }
        
        private void HandleDirectionMovement(Vector2 directionMovement) => _directionMovement = directionMovement;

        private void HandlePointerDelta(Vector2 delta) => _delta = delta;

        #endregion

        private void Update()
        {
            // Handle input based on the current movement type
            switch (MovementType)
            {
                case InputMovementTypes.WASD_MOUSE_TO_ROTATE:
                    HandleWasdMouseToRotate(_directionMovement, _delta);
                    break;
                case InputMovementTypes.MOUSE_ONLY:
                    HandleMouseOnly(_delta);
                    break;
                case InputMovementTypes.WASD_ONLY:
                    HandleWasdOnly(_directionMovement);
                    break;
            }
        }
        
        private void HandleWasdMouseToRotate(Vector2 directionMovement, Vector2 delta)
        {
            Vector3 movement = new Vector3(directionMovement.x, 0, directionMovement.y);
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            
            _characterController.Move(movement * (movementSpeed * Time.deltaTime));
            
            // Rotates with mouse
            firstPersonCamera.RotateCamera(delta);
        }

        private void HandleWasdOnly(Vector2 directionMovement)
        {
            // Movement direction based on transform.forward
            Vector3 movement = transform.forward * directionMovement.y;
            movement = movement.normalized;

            if (directionMovement.y < 0)
                movement *= _walkBackMovementPenalty; // Move by half speed, when walking back

            _characterController.Move(movement * (movementSpeed * Time.deltaTime));
            
            // Rotate based on A/D keys
            transform.Rotate(0, directionMovement.x * (rotationSpeedForWasdOnly * Time.deltaTime), 0);
        }

        
        private void HandleMouseOnly(Vector2 delta)
        {
            // Uses Astar to walk, and first person camera to look around.
            firstPersonCamera.RotateCamera(delta);
        }
        
        private void HandleLeftClickPathfinding()
        {
            if (MovementType != InputMovementTypes.MOUSE_ONLY) return;
            
            if (_astarAI == null) throw new UnityException("No IAstar found in the player controller prefab");
         
            _astarAI.destination = aiTargetMoveTowards.position;
        }
    }
}