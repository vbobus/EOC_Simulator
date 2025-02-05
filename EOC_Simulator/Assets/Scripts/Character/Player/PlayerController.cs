using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using NUnit.Framework;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Player
{
    public enum InputMovementTypes
    {
        WASD_MOUSE_TO_ROTATE, // WASD to move and Mouse will be used to rotate 
        MOUSE_ONLY            // Click to move and right click to rotate
    }

    public class PlayerController : Character
    {
        #region Variables
        // Movement type for the player (e.g., WASD + mouse, mouse only, etc.)
        [HideInInspector] public InputMovementTypes movementType;

        // Required references for the player controller
        [Required] [SerializeField] private Animator animator; // Handles player animations
        [Required] [SerializeField] private FirstPersonCamera firstPersonCamera; // Handles camera rotation
        [SerializeField] private LayerMask collisionLayerMask; // Layer mask for collision detection

        // Rotation speed for WASD-only movement type
        [Title("MovementType", "WASD Only")]
        [SerializeField] private float rotationSpeedForWasdOnly = 0.8f;

        // Character controller for movement and collision
        private CharacterController _characterController;

        // Input values for movement and camera rotation
        private Vector2 _directionMovement; // Stores WASD input
        private Vector2 _delta; // Stores mouse delta for camera rotation
        private float _lastDirectionInputTime; // Tracks the last time movement input was received

        // Pathfinding-related variables
        [Title("MovementType", "Pathfinding")]
        
        [InfoBox("Need to have a target prefab, to be able to use the pathfinding movement type!")]
        [SerializeField] private Transform aiTargetMoveTowards; // Target position for AI movement
        [SerializeField] private GameObject aiEndDestinationPrefab;

        #endregion

        #region SetUp

        protected override void Awake()
        {
            base.Awake();
            
            // Get required components
            _characterController = GetComponent<CharacterController>();
            
            // Subscribe to input events
            InputManager.Instance.On4DirectionMoveActionPressed += HandleDirectionMovement;
            InputManager.Instance.OnPointerDelta += HandlePointerDelta;
            InputManager.Instance.OnLeftClickActionPressed += HandleLeftClickPathfinding;
            InputManager.Instance.OnInteractActionPressed += TestChangeMovement;

            // Set initial movement type
            ChangeMovementType(InputMovementTypes.MOUSE_ONLY);
        }

        /// Test method to cycle through movement types
        private void TestChangeMovement()
        {
            int current = (int)movementType + 1;
            if (current >= Enum.GetValues(typeof(InputMovementTypes)).Length)
                current = 0;
            ChangeMovementType((InputMovementTypes)current);
        }

        /// Changes the player's movement type and updates related settings
        public void ChangeMovementType(InputMovementTypes newMovementType)
        {
            bool isMouseOnly = newMovementType == InputMovementTypes.MOUSE_ONLY;
            // Show/hide the AI target object if the field has been set


            if (aiTargetMoveTowards)
            {
                aiEndDestinationPrefab.SetActive(isMouseOnly);
                aiTargetMoveTowards.gameObject.SetActive(isMouseOnly);
            }
            else
            {
                // We want to change the movement type again, since we can't use the Mouse Only type when there is no target
                if (newMovementType == InputMovementTypes.MOUSE_ONLY) 
                    newMovementType = InputMovementTypes.WASD_MOUSE_TO_ROTATE;
            }
            
            movementType = newMovementType;
            firstPersonCamera.ResetCamera(); // Reset camera to default state

            // Stop pathfinding if not in MOUSE_ONLY mode
            if (AstarAI == null) return;

            AstarAI.isStopped = !isMouseOnly; // Stop or resume pathfinding
            AstarAI.destination = transform.position; // Reset destination

            // Enable/disable AIPath component based on movement type
            AIPath aiPath = AstarAI as AIPath;
            if (aiPath != null) aiPath.enabled = isMouseOnly;
        }

        /// Unsubscribe from input events when the object is destroyed
        private void OnDestroy()
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.On4DirectionMoveActionPressed -= HandleDirectionMovement;
            InputManager.Instance.OnPointerDelta -= HandlePointerDelta;
        }

        /// Handles 4-directional movement input (WASD)
        private void HandleDirectionMovement(Vector2 directionMovement) => _directionMovement = directionMovement;

        /// Handles mouse delta input for camera rotation
        private void HandlePointerDelta(Vector2 delta) => _delta = delta;

        #endregion

        private void Update()
        {
            // Handle input based on the current movement type
            switch (movementType)
            {
                case InputMovementTypes.WASD_MOUSE_TO_ROTATE:
                    HandleWasdMouseToRotate(_directionMovement, _delta);
                    break;
                case InputMovementTypes.MOUSE_ONLY:
                    HandleMouseOnly(_delta);
                    CheckAstarMovement(); // Update pathfinding movement
                    break;
            }
        }

        /// Handles movement for WASD + mouse rotation mode
        private void HandleWasdMouseToRotate(Vector2 directionMovement, Vector2 delta)
        {
            UpdateWalkIdleAnimations(directionMovement); // Update animations based on input

            // Calculate movement direction and move the player
            Vector3 movement = new Vector3(directionMovement.x, 0, directionMovement.y);
            movement = transform.TransformDirection(movement);
            movement.Normalize();
            
            
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            
            _characterController.Move(movement * (movementSpeed * Time.deltaTime));
            
            Velocity = _characterController.velocity;

            // Rotate the camera based on mouse input
            firstPersonCamera.RotateCamera(delta);
        }

        /// Updates walk/idle animations based on movement input
        private void UpdateWalkIdleAnimations(Vector2 directionMovement)
        {
            if (directionMovement == Vector2.zero)
            {
                // Switch to idle animation if no input is received for a short time
                if (Time.time - _lastDirectionInputTime >= DirectionIdleInputTime)
                {
                    animator.SetBool(AnimIsWalking, false);
                    animator.SetFloat(AnimInputX, 0f);
                    animator.SetFloat(AnimInputY, 0f);
                }
            }
            else
            {
                // Update walk animation based on input direction
                _lastDirectionInputTime = Time.time;
                animator.SetBool(AnimIsWalking, true);

                float currentInputX = animator.GetFloat(AnimInputX);
                float currentInputY = animator.GetFloat(AnimInputY);

                // Snap small input values to zero to avoid jittery animations
                if (Math.Abs(currentInputX) <= AnimThreshold) currentInputX = 0;
                if (Math.Abs(currentInputY) <= AnimThreshold) currentInputY = 0;

                // Smoothly interpolate input values for animations
                currentInputX = Mathf.Lerp(currentInputX, directionMovement.x, SmoothTime);
                currentInputY = Mathf.Lerp(currentInputY, directionMovement.y, SmoothTime);

                animator.SetFloat(AnimInputX, currentInputX);
                animator.SetFloat(AnimInputY, currentInputY);
            }
        }

        /// Handles movement for mouse-only mode (uses pathfinding)
        private void HandleMouseOnly(Vector2 delta)
        {
            // Rotate the camera based on mouse input
            firstPersonCamera.RotateCamera(delta);
        }

        /// Handles left-click input for setting a new pathfinding destination
        private void HandleLeftClickPathfinding()
        {
            if (movementType != InputMovementTypes.MOUSE_ONLY) return;
            // Set the AI destination to the target position
            aiEndDestinationPrefab.transform.position = aiTargetMoveTowards.position;
            SetDestination(aiTargetMoveTowards.position);
        }

    }
}