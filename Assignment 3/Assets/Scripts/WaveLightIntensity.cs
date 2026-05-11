using UnityEngine;

/// <summary>
/// Drives a Light's intensity (and optionally color) based on the current
/// wave number. Use this on the Directional Light or a key spotlight to make
/// later waves feel more intense / hostile.
/// </summary>
[RequireComponent(typeof(Light))]
public class WaveLightIntensity : MonoBehaviour
{
    [SerializeField] private float baseIntensity = 1.0f;
    [SerializeField] private float intensityPerWave = 0.15f;
    [SerializeField] private float maxIntensity = 2.5f;

    [Header("Optional color shift")]
    [SerializeField] private bool shiftColor = true;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = new Color(1f, 0.7f, 0.6f);
    [Tooltip("Wave at which color reaches endColor.")]
    [SerializeField] private int colorMaxWave = 8;

    [Tooltip("Lerp speed for smooth transitions.")]
    [SerializeField] private float lerpSpeed = 2f;

    private Light targetLight;
    private float targetIntensity;
    private Color targetColor;

    void Awake()
    {
        targetLight = GetComponent<Light>();
        targetIntensity = baseIntensity;
        targetColor = startColor;
    }

    void OnEnable()
    {
        GameManager.OnWaveChanged += HandleWaveChanged;
        GameManager.OnGameStarted += HandleGameStarted;
    }

    void OnDisable()
    {
        GameManager.OnWaveChanged -= HandleWaveChanged;
        GameManager.OnGameStarted -= HandleGameStarted;
    }

    void Update()
    {
        targetLight.intensity = Mathf.Lerp(targetLight.intensity, targetIntensity, Time.deltaTime * lerpSpeed);
        if (shiftColor)
            targetLight.color = Color.Lerp(targetLight.color, targetColor, Time.deltaTime * lerpSpeed);
    }

    private void HandleWaveChanged(int wave)
    {
        targetIntensity = Mathf.Min(baseIntensity + (wave - 1) * intensityPerWave, maxIntensity);
        if (shiftColor)
        {
            float t = Mathf.Clamp01((wave - 1f) / Mathf.Max(1, colorMaxWave - 1));
            targetColor = Color.Lerp(startColor, endColor, t);
        }
    }

    private void HandleGameStarted()
    {
        targetIntensity = baseIntensity;
        targetColor = startColor;
    }
}
