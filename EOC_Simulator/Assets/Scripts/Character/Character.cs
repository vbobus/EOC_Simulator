using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    [Serializable]
    public enum CharacterStates
    {
        IDLE,
        WALKING,
        DIALOGUE,
        INTERACTING
    }
    
    public abstract class Character : MonoBehaviour
    { 
        [SerializeField] protected float movementSpeed = 3.0f; // Base movement speed of the player
        protected IAstarAI AstarAI; // Reference to the A* pathfinding component
        public CharacterStates State { get; protected set; }
        
        // List of waypoints for pathfinding
        protected readonly List<Vector3> _waypoints = new List<Vector3>();
        
        // Animation-related variables
        protected Animator Animator;
        protected readonly float SmoothTime = 0.2f; // Smoothing time for animation transitions
        protected readonly float DirectionIdleInputTime = 0.1f; // Delay before switching to idle animation
        protected readonly float AnimThreshold = 0.05f; // Threshold for considering input values as zero
        
        // Animator parameter hashes for performance optimization
        protected static readonly int AnimIsWalking = Animator.StringToHash("IsWalking");
        protected static readonly int AnimInputX = Animator.StringToHash("InputX");
        protected static readonly int AnimInputY = Animator.StringToHash("InputY");
        
        // Movement speed penalty when the player looks away from the movement direction
        [SerializeField] [Range(50, 100)]
        private int lookAwayMovementSpeedPenalty = 75; // 50 -> 50% movement speed, 100 -> No penalty

        protected bool IsMoving;
        public Vector3 Velocity { get; protected set; }
        
        // Character controller for movement and collision
        protected CharacterController _characterController;
        
        // Gravity settings
        private const float Gravity = -9.81f; // Default gravity value
        [SerializeField] private float groundCheckDistance = 0.2f; // Distance to check for ground
        [SerializeField] private LayerMask groundLayer; // Layer mask for ground detectio
        // Gravity-related variables
        protected Vector3 _velocity; // Tracks vertical velocity (for gravity) Otherwise the character controller from Unity will handle the movement
        private bool _isGrounded; // Tracks if the player is on the ground
        
        protected virtual void Awake()
        {
            AstarAI = GetComponent<IAstarAI>();
            Animator = GetComponentInChildren<Animator>();
            _characterController = GetComponent<CharacterController>();
        }
        
        protected void SetDestination(Vector3 destination) => AstarAI.destination = destination;
        
        protected virtual void CheckAstarMovement()
        {
            if (AstarAI == null) return;
            
            // Get the remaining path from the A* AI component
            _waypoints.Clear();
            AstarAI.GetRemainingPath(_waypoints, out _);

            // Update walk animation based on AI velocity
            Animator.SetBool(AnimIsWalking, AstarAI.velocity.magnitude > AnimThreshold);

            if (_waypoints.Count <= 1) return;

            // Calculate the direction to the next waypoint and transform it into local space
            Vector3 directionToWaypoint = (_waypoints[1] - transform.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(directionToWaypoint);

            // Update animator parameters based on the local direction
            Animator.SetFloat(AnimInputX, localDirection.x);
            Animator.SetFloat(AnimInputY, localDirection.z);

            // Apply movement speed penalty if the player is looking away from the movement direction
            float t = Math.Clamp(localDirection.z + 1f, 0, 1f);
            float speedLerp = Mathf.Lerp(movementSpeed * (lookAwayMovementSpeedPenalty / 100f), movementSpeed, t);
            AstarAI.maxSpeed = speedLerp;

            Velocity = AstarAI.velocity;
        }

        protected virtual void Update()
        {
            CheckGrounded(); // Check if the player is on the ground
            ApplyGravity(); // Apply gravity to the player
        }

        private void LateUpdate()
        {
            if (State == CharacterStates.DIALOGUE || State == CharacterStates.INTERACTING) return;
            
            State = Velocity.magnitude < 0.1f ? CharacterStates.IDLE : CharacterStates.WALKING;
        }

        
        /// Checks if the player is grounded
        protected void CheckGrounded()
        {
            _isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundLayer);
        }

        /// Applies gravity to the player
        protected void ApplyGravity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                // Reset vertical velocity when grounded
                _velocity.y = -2f; // Small force to keep the player grounded
            }
            else
            {
                // Apply gravity when not grounded
                _velocity.y += Gravity * Time.deltaTime;
            }

            // Move the player vertically
            _characterController.Move(_velocity * Time.deltaTime);
        }

        /// Draws a gizmo to visualize the ground check
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
        }
    }
}