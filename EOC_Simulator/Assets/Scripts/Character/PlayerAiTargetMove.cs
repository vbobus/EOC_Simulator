using Events;
using UnityEngine;

namespace Character
{
    public class PlayerAiTargetMove : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float smoothSpeed = 10f; // Adjust for smoother movement

        private Vector3 _targetPosition;
        private Camera _mainCam;
        private void Awake()
        {
            InputManager.Instance.OnPointerPosition += PointerPositionUpdate;
            _mainCam = Camera.main;
            if (!_mainCam) 
                throw new UnityException($"Missing main camera");
        }

        private void PointerPositionUpdate(Vector2 pointerPosition)
        {
            if (!Application.isFocused) return;

            Ray ray = _mainCam.ScreenPointToRay(pointerPosition);        

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                _targetPosition = hit.point; // Store the target position
            }

            // Smoothly move the target towards the new position
            transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}