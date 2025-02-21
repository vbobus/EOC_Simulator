using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable
{
    public class PlayerCollideCheck : MonoBehaviour
    {
        public float colliderRadius = 2f;
        public bool shouldCheckForCollision;
        
        public UnityAction OnPlayerEnter;
        public UnityAction OnPlayerExit;

        public UnityEvent OnPlayerEnterEvent;
        private Collider[] _colliders;
        private bool _containsPlayer;
        
        private void Update()
        {
            if (!shouldCheckForCollision) return;
            
            Physics.OverlapSphereNonAlloc(transform.position, colliderRadius, _colliders, LayerMask.GetMask("Player"));
            // No player found
            if (_colliders.Length == 0)
            {
                if (!_containsPlayer) return;
                OnPlayerExit?.Invoke();
                _containsPlayer = false;
                return;
            }
            
            if (_containsPlayer) return;
            OnPlayerEnter?.Invoke();
            OnPlayerEnterEvent?.Invoke();
        }

        private void Awake()
        {
            OnPlayerEnter += () => { Debug.Log("Enter"); };
            OnPlayerExit += () => { Debug.Log("Exit"); };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, colliderRadius);
        }
    }
}
