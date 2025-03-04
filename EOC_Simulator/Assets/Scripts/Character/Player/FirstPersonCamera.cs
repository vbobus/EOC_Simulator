using System;
using System.Collections;
using Events;
using Unity.Cinemachine;
using UnityEngine;

namespace Character.Player
{
    using UnityEngine;

    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        private Transform _playerTransform; // Reference to the player's transform
        [SerializeField] private float sensitivity = 2; // Mouse sensitivity
        [SerializeField] private float smoothing = 1.5f; // Smoothing factor for mouse movement

        private Vector2 _cameraVelocity; // Accumulated camera rotation
        private Vector2 _frameVelocity; // Smoothed frame velocity
        
        [HideInInspector] public CinemachineCamera playerCamera;

        private float _minLockRotation = -50;
        private float _maxLockRotation = 30;
        
        private void Awake()
        {
            _playerTransform = playerController.transform;
            playerCamera = GetComponent<CinemachineCamera>();
            InputManager.Instance.OnSwitchedActionMap += SwitchedActionMap;
        }

        void Start()
        {
            ResetCamera();
        }
        
        private void SwitchedActionMap(ActionMap currentActionMap)
        {
            Debug.Log($"Switched to {currentActionMap}");
            if (currentActionMap == ActionMap.Player)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // StartCoroutine(ConfineCursor());
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            ResetCamera();
        }

        private IEnumerator ConfineCursor()
        {
            yield return new WaitForEndOfFrame(); // Wait for Unity to update
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        public void RotateCamera(Vector2 delta, bool rotatePlayerTransform = true)
        {
            // Don't rotate the camera, since it will make the frame data
            if (InputManager.Instance.ActionMap != ActionMap.Player) return;
            
            // Scale the delta by sensitivity and frame time
            Vector2 rawFrameVelocity = Vector2.Scale(delta, Vector2.one * (sensitivity * Time.deltaTime));

            // Smooth the frame velocity
            _frameVelocity = Vector2.Lerp(_frameVelocity, rawFrameVelocity, 1 / smoothing);

            // Add the smoothed frame velocity to the accumulated camera velocity
            _cameraVelocity += _frameVelocity;

            // Clamp the vertical rotation to avoid extreme angles
            _cameraVelocity.y = Mathf.Clamp(_cameraVelocity.y, _minLockRotation, _maxLockRotation);

            // Apply the vertical rotation to the camera (X-axis)
            transform.localRotation = Quaternion.AngleAxis(-_cameraVelocity.y, Vector3.right);
            
            // Apply the horizontal rotation to the player (Y-axis)
            if (rotatePlayerTransform)
                _playerTransform.localRotation = Quaternion.AngleAxis(_cameraVelocity.x, Vector3.up);
        }
    
        public void ResetCamera()
        {
            _frameVelocity = Vector2.zero;
        }
    }
}
