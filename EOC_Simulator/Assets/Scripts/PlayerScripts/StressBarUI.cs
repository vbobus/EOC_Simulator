using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class StressBarUI : MonoBehaviour
{
    [Header("🔹 UI Elements")]
    public Slider stressSlider;
    public Image fillImage;
    public PlayerStress playerStress;

    [Header("🎨 Color Transition")]
    private Color targetColor;
    private float lerpSpeed = 3f;

    [Header("🔹 Shake Effect")]
    private RectTransform stressBarTransform;
    private Vector3 originalPosition;
    private bool isShaking = false;

    [Header("🔊 Heartbeat Sound")]
    public AudioSource heartbeatAudio;
    public AnimationCurve shakeCurve; // 让抖动更自然

    [Header("🎥 Camera Shake")]
    public Transform playerCamera;
    private Vector3 originalCamPos;
    private bool isCameraShaking = false;

    [Header("🎯 Screen Blur Effect")]
    public Volume postProcessVolume;
    private DepthOfField depthOfField;
    // private float blurIntensity = 0f; // 模糊强度
    private bool isBlurred = false;

    void Start()
    {
        if (stressSlider == null) return;
        fillImage = stressSlider.fillRect.GetComponent<Image>();

        stressBarTransform = stressSlider.GetComponent<RectTransform>();
        originalPosition = stressBarTransform.localPosition;

        // 🎥 记录摄像机原始位置
        if (playerCamera != null)
            originalCamPos = playerCamera.localPosition;

        if (heartbeatAudio != null)
        {
            heartbeatAudio.volume = 0;
            heartbeatAudio.loop = true;
            heartbeatAudio.Play();
        }

        // 🎯 获取后处理的 Depth of Field 组件
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out depthOfField))
        {
            depthOfField.gaussianEnd.Override(0f); // 初始模糊度
        }
    }

    void Update()
    {
        if (playerStress == null) return;

        // 🎯 更新压力 UI
        stressSlider.value = playerStress.stressLevel;
        UpdateStressColor();
        UpdateStressEffects();
    }

    // 🎨 颜色平滑过渡：绿 → 黄 → 红
    private void UpdateStressColor()
    {
        float t = playerStress.stressLevel / 100f;

        if (t < 0.4f)
        {
            targetColor = Color.Lerp(Color.green, Color.yellow, t / 0.4f);
        }
        else if (t < 0.7f)
        {
            targetColor = Color.Lerp(Color.yellow, Color.red, (t - 0.4f) / 0.3f);
        }
        else
        {
            targetColor = Color.red;
        }

        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * lerpSpeed);
    }

    // 🎯 处理高压力效果（抖动、心跳、屏幕模糊、摄像机晃动）
    private void UpdateStressEffects()
    {
        float stress = playerStress.stressLevel / 100f;

        // 🎯 进入高压力（≥70%），触发抖动 & 心跳加速 & 屏幕模糊 & 摄像机晃动
        if (playerStress.stressLevel >= 70)
        {
            if (!isShaking)
            {
                isShaking = true;
                InvokeRepeating("ShakeBar", 0f, 0.05f);
            }

            if (!isCameraShaking && playerCamera != null)
            {
                isCameraShaking = true;
                StartCoroutine(ShakeCamera());
            }

            if (heartbeatAudio != null)
            {
                heartbeatAudio.volume = Mathf.Lerp(heartbeatAudio.volume, 1f, Time.deltaTime * 2f);
                heartbeatAudio.pitch = Mathf.Lerp(heartbeatAudio.pitch, 1.5f, Time.deltaTime * 2f);
            }

            // 🎯 只有在红色阶段才开始模糊
            if (!isBlurred)
            {
                isBlurred = true;
                StartCoroutine(BlurScreen(true));
            }
        }
        else
        {
            if (isShaking)
            {
                isShaking = false;
                CancelInvoke("ShakeBar");
                stressBarTransform.localPosition = originalPosition;
            }

            if (isCameraShaking)
            {
                isCameraShaking = false;
            }

            if (heartbeatAudio != null)
            {
                heartbeatAudio.volume = Mathf.Lerp(heartbeatAudio.volume, 0f, Time.deltaTime * 2f);
                heartbeatAudio.pitch = Mathf.Lerp(heartbeatAudio.pitch, 1f, Time.deltaTime * 2f);
            }

            // 🎯 当压力回落到黄或绿，移除模糊
            if (isBlurred && playerStress.stressLevel < 70)
            {
                isBlurred = false;
                StartCoroutine(BlurScreen(false));
            }
        }
    }

    // 🎯 HUD 抖动（用 Animation Curve 让抖动更真实）
    void ShakeBar()
    {
        if (stressBarTransform == null) return;

        float shakeAmount = shakeCurve.Evaluate(Time.time % 1f) * 5f; // 让抖动更平滑
        stressBarTransform.localPosition = originalPosition + new Vector3(shakeAmount, 0, 0);
    }

    // 🎥 摄像机晃动
    IEnumerator ShakeCamera()
    {
        while (isCameraShaking)
        {
            float shakeX = Random.Range(-0.1f, 0.1f);
            float shakeY = Random.Range(-0.1f, 0.1f);
            playerCamera.localPosition = originalCamPos + new Vector3(shakeX, shakeY, 0);
            yield return new WaitForSeconds(0.05f);
        }

        // 恢复摄像机原位置
        playerCamera.localPosition = originalCamPos;
    }

    // 🎯 屏幕模糊效果
    IEnumerator BlurScreen(bool blurIn)
    {
        float targetBlur = blurIn ? 5f : 0f; // 目标模糊度
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newBlur = Mathf.Lerp(depthOfField.gaussianEnd.value, targetBlur, elapsed / duration);
            depthOfField.gaussianEnd.Override(newBlur);
            yield return null;
        }
    }
}