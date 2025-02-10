using System;
using UnityEngine;

namespace QuestSystem
{
    public class CloseToQuestStep : QuestStep
    {
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layerMask;

        [SerializeField] private float timeBetweenChecks = .1f;
        private float _time;
        private void FixedUpdate()
        {
            if (status != QuestStepStatus.IS_ACTIVE) return;
            
            // To avoid this overlapSphere from running a lot more times than necessary 
            _time += Time.deltaTime;
            if (_time < timeBetweenChecks) return;
            
            _time = 0; // Resets the time
            
            var results = Physics.OverlapSphere(transform.position + positionOffset, radius, layerMask);
            foreach (var result in results)
            {
                TryGetComponent<Character.Player.PlayerController>(out var playerController);
                if (!playerController) continue;
                QuestComplete();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + positionOffset, radius);
        }
    }
}