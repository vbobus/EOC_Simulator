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

        
        
        [SerializeField] private string testAnimLayerLockName;
        
        // List of waypoints for pathfinding
        private readonly List<Vector3> _waypoints = new List<Vector3>();
        
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
        
        protected virtual void Awake()
        {
            AstarAI = GetComponent<IAstarAI>();
            Animator = GetComponentInChildren<Animator>();
            if (!String.IsNullOrEmpty(testAnimLayerLockName))
            {
                int layerIndex = Animator.GetLayerIndex(testAnimLayerLockName);
                Animator.SetLayerWeight(layerIndex, 1f);
            }
        }
        
        protected void SetDestination(Vector3 destination) => AstarAI.destination = destination;
        
        protected void CheckAstarMovement()
        {
            // Get the remaining path from the A* AI component
            _waypoints.Clear();
            AstarAI.GetRemainingPath(_waypoints, out bool hasFinishedPath);

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

        private void LateUpdate()
        {
            if (State == CharacterStates.DIALOGUE || State == CharacterStates.INTERACTING) return;
            
            State = Velocity.magnitude < 0.1f ? CharacterStates.IDLE : CharacterStates.WALKING;
            
            // if (Velocity != Vector3.zero) Debug.Log($"Velocity: {Velocity}");
        }
    }
}