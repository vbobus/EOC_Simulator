using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using NUnit.Framework;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Character.Player
{
    public enum InputMovementTypes
    {
        WASD_MOUSE_TO_ROTATE, // WASD to move and Mouse will be used to rotate 
        WASD_ONLY,            // Client idea of movement, WS => forward, back.   AD => rotate left or right
        MOUSE_ONLY            // Click to move and right click to rotate
    }

    public class PlayerController : Character
    {
        #region Variables

        // Movement type for the player (e.g., WASD + mouse, mouse only, etc.)
        public static InputMovementTypes MovementType;

        // Required references for the player controller
        [Required] [SerializeField] private Animator animator; // Handles player animations
        [Required] [SerializeField] private FirstPersonCamera firstPersonCamera; // Handles camera rotation
        [SerializeField] private LayerMask collisionLayerMask; // Layer mask for collision detection

        // Penalty for walking backward (reduces movement speed)
        private readonly float _walkBackMovementPenalty = 0.5f;

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
        [SerializeField] private Transform aiTargetMoveTowards; // Target position for AI movement

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
            int current = (int)MovementType + 1;
            if (current >= Enum.GetValues(typeof(InputMovementTypes)).Length)
                current = 0;
            ChangeMovementType((InputMovementTypes)current);
        }

        /// Changes the player's movement type and updates related settings
        public void ChangeMovementType(InputMovementTypes newMovementType)
        {
            MovementType = newMovementType;
            firstPersonCamera.ResetCamera(); // Reset camera to default state

            // Stop pathfinding if not in MOUSE_ONLY mode
            if (AstarAI == null) return;

            bool isMouseOnly = newMovementType == InputMovementTypes.MOUSE_ONLY;
            AstarAI.isStopped = !isMouseOnly; // Stop or resume pathfinding
            AstarAI.destination = transform.position; // Reset destination

            // Enable/disable AIPath component based on movement type
            AIPath aiPath = AstarAI as AIPath;
            if (aiPath != null) aiPath.enabled = isMouseOnly;

            // Show/hide the AI target object
            aiTargetMoveTowards.gameObject.SetActive(isMouseOnly);
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
            switch (MovementType)
            {
                case InputMovementTypes.WASD_MOUSE_TO_ROTATE:
                    HandleWasdMouseToRotate(_directionMovement, _delta);
                    break;
                case InputMovementTypes.MOUSE_ONLY:
                    HandleMouseOnly(_delta);
                    CheckAstarMovement(); // Update pathfinding movement
                    break;
                case InputMovementTypes.WASD_ONLY:
                    HandleWasdOnly(_directionMovement);
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
            _characterController.Move(movement * (movementSpeed * Time.deltaTime));

            // Rotate the camera based on mouse input
            firstPersonCamera.RotateCamera(delta);
        }

        /// Handles movement for WASD-only mode
        private void HandleWasdOnly(Vector2 directionMovement)
        {
            UpdateWalkIdleAnimations(directionMovement); // Update animations based on input

            // Calculate movement direction and apply backward movement penalty
            Vector3 movement = transform.forward * directionMovement.y;
            movement = movement.normalized;
            if (directionMovement.y < 0)
                movement *= _walkBackMovementPenalty; // Reduce speed when walking backward

            _characterController.Move(movement * (movementSpeed * Time.deltaTime));

            // Rotate the player based on A/D keys
            transform.Rotate(0, directionMovement.x * (rotationSpeedForWasdOnly * Time.deltaTime), 0);
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
            if (MovementType != InputMovementTypes.MOUSE_ONLY) return;
            // Set the AI destination to the target position
            SetDestination(aiTargetMoveTowards.position);
        }
    }
}