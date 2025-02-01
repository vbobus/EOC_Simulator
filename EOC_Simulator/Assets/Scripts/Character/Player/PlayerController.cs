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

    public class PlayerController : MonoBehaviour
    {
        #region Variables

        // Movement type for the player (e.g., WASD + mouse, mouse only, etc.)
        public static InputMovementTypes MovementType;

        // Required references for the player controller
        [Required] [SerializeField] private Animator animator; // Handles player animations
        [Required] [SerializeField] private FirstPersonCamera firstPersonCamera; // Handles camera rotation
        [SerializeField] private LayerMask collisionLayerMask; // Layer mask for collision detection
        [SerializeField] private float movementSpeed = 3.0f; // Base movement speed of the player

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

        // Pathfinding-related variables
        [Title("MovementType", "Pathfinding")]
        [SerializeField] private LayerMask aiWalkTargetLayerMask; // Layer mask for AI pathfinding targets
        [SerializeField] private Transform aiTargetMoveTowards; // Target position for AI movement
        private IAstarAI _astarAI; // Reference to the A* pathfinding component

        // Movement speed penalty when the player looks away from the movement direction
        [SerializeField] [UnityEngine.Range(50, 100)]
        private int lookAwayMovementSpeedPenalty = 75; // 50 -> 50% movement speed, 100 -> No penalty

        // List of waypoints for pathfinding
        private readonly List<Vector3> _waypoints = new List<Vector3>();

        // Animation-related variables
        private float _lastDirectionInputTime; // Tracks the last time movement input was received
        private readonly float _smoothTime = 0.2f; // Smoothing time for animation transitions
        private readonly float _directionIdleInputTime = 0.1f; // Delay before switching to idle animation
        private readonly float _animThreshold = 0.05f; // Threshold for considering input values as zero

        // Animator parameter hashes for performance optimization
        private static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
        private static readonly int AnimInputX = Animator.StringToHash("InputX");
        private static readonly int AnimInputY = Animator.StringToHash("InputY");

        #endregion

        #region SetUp

        private void Awake()
        {
            // Get required components
            _characterController = GetComponent<CharacterController>();
            _astarAI = GetComponent<IAstarAI>();
            _astarAI.maxSpeed = movementSpeed; // Set initial speed for pathfinding

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
            if (_astarAI == null) return;

            bool isMouseOnly = newMovementType == InputMovementTypes.MOUSE_ONLY;
            _astarAI.isStopped = !isMouseOnly; // Stop or resume pathfinding
            _astarAI.destination = transform.position; // Reset destination

            // Enable/disable AIPath component based on movement type
            AIPath aiPath = _astarAI as AIPath;
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
                if (Time.time - _lastDirectionInputTime >= _directionIdleInputTime)
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
                if (Math.Abs(currentInputX) <= _animThreshold) currentInputX = 0;
                if (Math.Abs(currentInputY) <= _animThreshold) currentInputY = 0;

                // Smoothly interpolate input values for animations
                currentInputX = Mathf.Lerp(currentInputX, directionMovement.x, _smoothTime);
                currentInputY = Mathf.Lerp(currentInputY, directionMovement.y, _smoothTime);

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

        /// Updates pathfinding movement and animations
        private void CheckAstarMovement()
        {
            // Get the remaining path from the A* AI component
            _waypoints.Clear();
            _astarAI.GetRemainingPath(_waypoints, out bool hasFinishedPath);

            // Update walk animation based on AI velocity
            animator.SetBool(AnimIsWalking, _astarAI.velocity.magnitude > _animThreshold);

            if (_waypoints.Count <= 1) return;

            // Calculate the direction to the next waypoint and transform it into local space
            Vector3 directionToWaypoint = (_waypoints[1] - transform.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(directionToWaypoint);

            // Update animator parameters based on the local direction
            animator.SetFloat(AnimInputX, localDirection.x);
            animator.SetFloat(AnimInputY, localDirection.z);

            // Apply movement speed penalty if the player is looking away from the movement direction
            float t = Math.Clamp(localDirection.z + 1f, 0, 1f);
            float speedLerp = Mathf.Lerp(movementSpeed * (lookAwayMovementSpeedPenalty / 100f), movementSpeed, t);
            _astarAI.maxSpeed = speedLerp;
        }

        /// Handles left-click input for setting a new pathfinding destination
        private void HandleLeftClickPathfinding()
        {
            if (MovementType != InputMovementTypes.MOUSE_ONLY) return;

            if (_astarAI == null) throw new UnityException("No IAstar found in the player controller prefab");

            // Set the AI destination to the target position
            _astarAI.destination = aiTargetMoveTowards.position;
        }
    }
}