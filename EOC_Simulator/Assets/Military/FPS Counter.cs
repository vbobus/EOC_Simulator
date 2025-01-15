using UnityEngine;
using System.Collections.Generic;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private List<float> fpsReadings = new List<float>();
    private float averageFPS = 0.0f;
    private float timeElapsed = 0.0f;

    void Update()
    {
        // Update delta time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate current FPS
        float currentFPS = 1.0f / deltaTime;

        // Add FPS reading to the list
        fpsReadings.Add(currentFPS);
        timeElapsed += Time.unscaledDeltaTime;

        // Keep only the last 10 seconds of readings
        while (timeElapsed > 10.0f)
        {
            timeElapsed -= Time.unscaledDeltaTime;
            fpsReadings.RemoveAt(0); // Remove the oldest reading
        }

        // Calculate average FPS
        averageFPS = 0.0f;
        foreach (float fps in fpsReadings)
        {
            averageFPS += fps;
        }
        averageFPS /= fpsReadings.Count;
    }

    void OnGUI()
    {
        // Current FPS
        float currentFPS = 1.0f / deltaTime;
        string currentFPSText = $"Current FPS: {currentFPS:0.}";

        // Average FPS
        string averageFPSText = $"Average FPS (10s): {averageFPS:0.}";

        // Display FPS on the screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        Rect currentFPSRect = new Rect(10, 10, 300, 30);
        Rect averageFPSRect = new Rect(10, 40, 300, 30);

        GUI.Label(currentFPSRect, currentFPSText, style);
        GUI.Label(averageFPSRect, averageFPSText, style);
    }
}