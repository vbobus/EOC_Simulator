using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using NUnit.Framework;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;
using PixelCrushers.DialogueSystem;
using QuickOutline.Scripts;
using UnityEngine.Events;


namespace Character.Player
{
    [Serializable]
    public enum InputMovementTypes
    {
        WASD_MOUSE_TO_ROTATE, // WASD to move and Mouse will be used to rotate 
        MOUSE_ONLY            // Click to move and right click to rotate
    }

    public class PlayerController : Character
    {
        #region Variables
        // Movement type for the player (e.g., WASD + mouse, mouse only, etc.)
        public static InputMovementTypes MovementType { get; private set; }

        // Required references for the player controller
        [SerializeField] private Animator animator; // Handles player animations
        [SerializeField] private FirstPersonCamera firstPersonCamera; // Handles camera rotation
        [SerializeField] private LayerMask collisionLayerMask; // Layer mask for collision detection

        // Input values for movement and camera rotation
        private Vector2 _directionMovement; // Stores WASD input
        private Vector2 _delta; // Stores mouse delta for camera rotation
        private float _lastDirectionInputTime; // Tracks the last time movement input was received

        // Pathfinding-related variables
        // "Need to have a target prefab, to be able to use the pathfinding movement type!"
        [Header("Pathfinding")]
        [SerializeField] private Transform aiTargetMoveTowards; // Target position for AI movement
        [SerializeField] private GameObject aiEndDestinationPrefab;
        public bool CanAstarMove { get; set; } = true;
        
        public bool CanMoveWithAstar()
        {
            return !InputManager.Instance.ActionMapIsUI() && CanAstarMove && CanMove;
        }

        
        // On canmove false, stop the velocity.
        public bool CanMove { get; set; } = true;
        
        #endregion

        public static Transform playerTransform;
        
        #region SetUp

        protected override void Awake()
        {
            base.Awake();
            
            playerTransform = transform;

            // Get required components
            _characterController = GetComponent<CharacterController>();

            // Subscribe to input events
            InputManager.Instance.On4DirectionMoveActionPressed += HandleDirectionMovement;
            InputManager.Instance.OnPointerDelta += HandlePointerDelta;
            InputManager.Instance.OnLeftClickActionPressed += HandleLeftClickPathfinding;
            // InputManager.Instance.OnInteractActionPressed += TestChangeMovement;
            InputManager.Instance.OnSwitchedActionMap += SwitchedActionMap;
            // Set initial movement type
            ChangeMovementType(InputMovementTypes.MOUSE_ONLY);
        }
        
        private void SwitchedActionMap(ActionMap newActionMap)
        {
            if (newActionMap == ActionMap.Player)
            {
                CanMove = true;
            }
            else
            {
                CanMove = false;
                // Set the input to be reset.
                _directionMovement = Vector2.zero;
                _delta = Vector2.zero;
            }
            AstarAI.canMove = CanMove;
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

            MovementType = newMovementType;
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

        protected override void Update()
        {
            base.Update();

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
            }
        }

        protected override void CheckAstarMovement()
        {
            if (InputManager.Instance.ActionMapIsUI())
            {
                StopWalkingAnimation();
                return;
            }
            base.CheckAstarMovement();
        }

        /// Handles movement for WASD + mouse rotation mode
        private void HandleWasdMouseToRotate(Vector2 directionMovement, Vector2 delta)
        {
            UpdateWalkIdleAnimations(directionMovement); // Update animations based on input

            if (!CanMove) return;
            
            // Calculate movement direction and move the player
            Vector3 movement = new Vector3(directionMovement.x, 0, directionMovement.y);
            movement = transform.TransformDirection(movement);
            movement.Normalize();

            // Apply movement and gravity
            _characterController.Move(movement * (movementSpeed * Time.deltaTime) + _velocity * Time.deltaTime);

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
                    StopWalkingAnimation();
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

        private void StopWalkingAnimation()
        {
            animator.SetBool(AnimIsWalking, false);
            animator.SetFloat(AnimInputX, 0f);
            animator.SetFloat(AnimInputY, 0f);
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
            if (MovementType != InputMovementTypes.MOUSE_ONLY || !CanMoveWithAstar()) return;
            
            // Set the AI destination to the target position
            aiEndDestinationPrefab.transform.position = aiTargetMoveTowards.position;
            SetDestination(aiTargetMoveTowards.position);
        }

    }
}