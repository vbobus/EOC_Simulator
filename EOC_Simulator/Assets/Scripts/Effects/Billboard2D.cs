using UnityEngine;

namespace Effects
{
    public class Billboard2D : MonoBehaviour
    {
        private Camera _mainCamera;

        void Start()
        {
            // Find the main camera in the scene
            _mainCamera = Camera.main;

            if (_mainCamera == null)
                Debug.LogError("No main camera found in the scene!");
        }

        void Update()
        {
            if (!_mainCamera) return;
            
            FaceCamera();
        }

        void FaceCamera()
        {
            // Get the direction from the sprite to the camera
            Vector3 directionToCamera = _mainCamera.transform.position - transform.position;

            // Zero out the Y component to keep the sprite upright (optional)
            directionToCamera.y = 0;

            // Rotate the sprite to face the camera
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }
}
