using System;
using System.Collections;
using Events;
using PixelCrushers;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Character.Player
{
    using UnityEngine;

    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform playerTransform; // Reference to the player's transform
        [SerializeField] private float sensitivity = 2; // Mouse sensitivity
        [SerializeField] private float smoothing = 1.5f; // Smoothing factor for mouse movement

        private Vector2 _cameraVelocity; // Accumulated camera rotation
        private Vector2 _frameVelocity; // Smoothed frame velocity
        
        [HideInInspector] public CinemachineCamera playerCamera;

        private float _minLockRotation = -50;
        private float _maxLockRotation = 30;
        
        private void Awake()
        {
            playerCamera = GetComponent<CinemachineCamera>();
            InputManager.Instance.OnSwitchedActionMap += SwitchedActionMap;
            InputManager.Instance.OnPointerDelta += OnUpdateDelta;
        }

        private Vector2 _delta;
        
        private void OnUpdateDelta(Vector2 arg0)
        {
            _delta = arg0;
        }

        void Start()
        {
            SwitchedActionMap(InputManager.Instance.ActionMap);
        }
        
        private ActionMap _currentActionMap;
        
        private void SwitchedActionMap(ActionMap currentActionMap)
        {
            // Debug.Log($"Switched to {currentActionMap}");
            _currentActionMap = currentActionMap;
            
            // if (currentActionMap == ActionMap.Player)
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            //     // Cursor.visible = false;
            // }
            // else
            // {
            //     Cursor.lockState = CursorLockMode.None;
            //     // Cursor.visible = true;
            // }
            
            CursorControl.LockCursor(currentActionMap == ActionMap.Player);
            
            ResetCamera();
        }

        
        // TO DO : Change to on event focuses in the game. Then check
        // private void Update()
        // {
        //     if (_currentActionMap != ActionMap.Player) return;
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }

        private void LateUpdate()
        {
            RotateCamera(_delta);
        }
        
        
        public void RotateCamera(Vector2 delta, bool rotatePlayerTransform = true)
        {
            if (!Application.isFocused || InputManager.Instance.ActionMap != ActionMap.Player)
            {
                ResetCamera();
                return;
            }
            
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
                playerTransform.localRotation = Quaternion.AngleAxis(_cameraVelocity.x, Vector3.up);
        }
    
        public void ResetCamera()
        {
            _frameVelocity = Vector2.zero;
        }
        
        private bool IsMouseInsideScreen()
        {
            Vector3 mousePos = Input.mousePosition;
            return mousePos.x >= 0 && mousePos.x <= Screen.width &&
                   mousePos.y >= 0 && mousePos.y <= Screen.height;
        }
    }
}
