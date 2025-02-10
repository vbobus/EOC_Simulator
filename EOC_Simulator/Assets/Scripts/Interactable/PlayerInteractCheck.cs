using System;
using Events;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactable
{
    public class PlayerInteractCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private CinemachineCamera playerCamera;
        
        // [Range(1f, 3f)]
        [SerializeField] private float boxLenght = 1.0f;
        //
        // [Range(0.1f, 0.5f)]
        // [SerializeField] private float boxWidth = 0.3f;

        // [SerializeField] private Vector3 _boxSize;
        // private float _boxLenght => boxLenght / 2;

        private InteractableObject _lastInteractableObject;

        [SerializeField] private Character.Player.PlayerController playerController;
        private void OnEnable()
        {
            InputManager.Instance.OnLeftClickActionPressed += InteractWithLastSeenInteractable;
        }

        private void InteractWithLastSeenInteractable()
        {
            if (!_lastInteractableObject) return;
            _lastInteractableObject.Interact();
        }

        private void FixedUpdate()
        {
            if (InputManager.Instance.ActionMapIsUI())
            {
                if (_lastInteractableObject) _lastInteractableObject = null;
                return;
            }
            
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, boxLenght, interactableLayer))
            {
                // Debug.Log("Hit: " + hit.collider.name);
                InteractableObject interactableObject = hit.collider.gameObject.GetComponent<InteractableObject>();

                if (!_lastInteractableObject && interactableObject)
                {
                    interactableObject.OnHoverOver();
                }
                
                _lastInteractableObject = interactableObject;
                playerController.CanAstarMove = false;
            }
            else
            {
                if (_lastInteractableObject)
                {
                    _lastInteractableObject.OnHoverOut();
                }
                _lastInteractableObject = null;
                playerController.CanAstarMove = true;
            }
        }
        
        /*
         *             bool hit = Physics.BoxCast(playerCamera.transform.position,
                _boxSize / 2,
                playerCamera.transform.forward,
                out RaycastHit hitInfo,
                playerCamera.transform.rotation,
                _boxLenght,
                interactableLayer);

         */
        
        private void OnDrawGizmos()
        {
            // Set the color of the Gizmos
            Gizmos.color = Color.green;

            Gizmos.DrawLine(playerCamera.transform.position, playerCamera.transform.position + playerCamera.transform.forward * boxLenght);
        }
    }
}