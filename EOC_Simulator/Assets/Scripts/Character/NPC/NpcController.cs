using System;
using UnityEngine;

namespace Character.NPC
{
    public class NpcController : Character
    {
        [SerializeField] private string testAnimLayerLockName;

        
        [SerializeField] private Transform walkTargetTransform;

        protected override void Awake()
        {
            base.Awake();
            
            if (!String.IsNullOrEmpty(testAnimLayerLockName))
            {
                int layerIndex = Animator.GetLayerIndex(testAnimLayerLockName);
                Animator.SetLayerWeight(layerIndex, 1f);
            }
        }

        private void Start()
        {
            
        }

        protected override void Update()
        {
            // base.Update();
            
            if (!walkTargetTransform) return;
            SetDestination(walkTargetTransform.position);
            
            // Update walk animation based on AI velocity
            Animator.SetBool(AnimIsWalking, AstarAI.velocity.magnitude > AnimThreshold);

            AstarAI.GetRemainingPath(_waypoints, out _);
            
            if (_waypoints.Count <= 1) return;

            // Calculate the direction to the next waypoint and transform it into local space
            Vector3 directionToWaypoint = (_waypoints[1] - transform.position).normalized;
            Vector3 localDirection = transform.InverseTransformDirection(directionToWaypoint);

            // Update animator parameters based on the local direction
            Animator.SetFloat(AnimInputX, localDirection.x);
            Animator.SetFloat(AnimInputY, localDirection.z);
        }
    }
}