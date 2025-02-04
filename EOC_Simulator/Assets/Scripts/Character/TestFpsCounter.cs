using TMPro;
using UnityEngine;

namespace Character
{
    // Generated with ai
    public class TestFpsCounter : MonoBehaviour
    { 
        public TextMeshProUGUI fpsText; // Reference to the TextMeshPro component

        private float[] fpsBuffer; // Buffer to store FPS values for the last 5 seconds
        private int bufferIndex = 0; // Current index in the buffer
        private float timer = 0f; // Timer to track when to update the buffer
        private int triangleCount = 0; // Number of triangles drawn in the current frame

        void Start()
        {
            if (fpsText == null)
            {
                Debug.LogError("No TextMeshProUGUI component assigned!");
                enabled = false; // Disable the script if no TMP component is assigned
                return;
            }

            // Initialize the FPS buffer for the last 5 seconds
            fpsBuffer = new float[Mathf.CeilToInt(5f / Time.fixedDeltaTime)];
        }

        void Update()
        {
            // Calculate current FPS
            float currentFPS = 1f / Time.unscaledDeltaTime;

            // Update the FPS buffer
            if (timer >= Time.fixedDeltaTime)
            {
                fpsBuffer[bufferIndex] = currentFPS;
                bufferIndex = (bufferIndex + 1) % fpsBuffer.Length;
                timer = 0f;
            }
            timer += Time.deltaTime;

            // Calculate average FPS over the last 5 seconds
            float averageFPS = 0f;
            for (int i = 0; i < fpsBuffer.Length; i++)
            {
                averageFPS += fpsBuffer[i];
            }
            averageFPS /= fpsBuffer.Length;

            // Get the number of triangles drawn in the current frame
            // triangleCount = GetTriangleCount();

            // Update the TMP text
            fpsText.text = $"FPS: {currentFPS:F1}\nAvg FPS (5s): {averageFPS:F1}";
        }

        // Helper function to get the number of triangles drawn in the current frame
        private int GetTriangleCount()
        {
            int triangles = 0;
            foreach (var filter in FindObjectsOfType<MeshFilter>())
            {
                if (filter.sharedMesh != null)
                {
                    triangles += filter.sharedMesh.triangles.Length / 3;
                }
            }
            return triangles;
        }
    }
}