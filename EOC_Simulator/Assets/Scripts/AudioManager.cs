using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance (optional, if you want to access it from anywhere)
    public static AudioManager Instance { get; private set; }

    // Reference to the AudioSource that plays the background sfx
    [SerializeField] private AudioSource backgroundAudioSource;

    private void Awake()
    {
        // Simple singleton implementation (optional)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure the AudioSource is set to loop and play on start
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.loop = true;
            backgroundAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Background AudioSource not assigned in AudioManager.");
        }
    }

    /// <summary>
    /// Call this method to turn the background audio on or off.
    /// </summary>
    /// <param name="isOn">If true, audio will play; if false, audio will be muted.</param>
    public void SetMusicOn(bool isOn)
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.mute = !isOn;
        }
    }
}