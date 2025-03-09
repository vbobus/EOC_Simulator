using System;
using Events;
using UnityEngine;

namespace Character.Player
{
    public class PlayerAiTargetMove : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float maxDistanceRayCheck = 10f;
        private Vector3 _targetPosition;
        private Camera _mainCam;

        // If the player is in UI move, or if cursor is close to interactable
        [HideInInspector] public bool CanMove = true;
        [SerializeField] private PlayerController playerController;
        
        private void Awake()
        {
            // InputManager.Instance.OnPointerPosition += PointerPositionUpdate;
            _mainCam = Camera.main;
            if (!_mainCam) 
                throw new UnityException($"Missing main camera");
        }

        private void Update()
        {
            PointerPositionUpdate(new Vector2(Screen.width / 2f, Screen.height / 2f));
        }

        private void PointerPositionUpdate(Vector2 pointerPosition)
        {
            if (!playerController.CanMoveWithAstar())
            {
                return;
            }
            if (!Application.isFocused) return;

            Ray ray = _mainCam.ScreenPointToRay(pointerPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistanceRayCheck, layerMask)) return;
            
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _targetPosition = hit.point; 
            }
            else
            {
                // Raycast down, to hit the ground, then have the target pos be that. 
                Ray wallToGroundRay = new Ray(hit.point, -Vector3.up);
                Physics.Raycast(wallToGroundRay, out hit, maxDistanceRayCheck, LayerMask.GetMask("Ground"));
                _targetPosition = hit.point;
            }

            transform.position = _targetPosition;
        }
    }
}