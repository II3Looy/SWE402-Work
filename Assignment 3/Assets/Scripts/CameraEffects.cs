using UnityEngine;

/// <summary>
/// Camera FOV controller:
///  - Smooth FOV pulse while the powerup is active.
///  - Smooth zoom-out as wave number increases (small per-wave step).
/// Attach to the Main Camera GameObject.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    [Header("Base FOV")]
    [Tooltip("Starting field of view (degrees).")]
    [SerializeField] private float baseFov = 60f;

    [Header("Powerup pulse")]
    [Tooltip("How much the FOV widens while powered up.")]
    [SerializeField] private float powerupFovBoost = 8f;
    [Tooltip("How fast the FOV transitions in/out of powerup state.")]
    [SerializeField] private float pulseSpeed = 4f;

    [Header("Per-wave zoom-out")]
    [Tooltip("FOV added per wave above wave 1 (caps at maxWaveBoost).")]
    [SerializeField] private float fovPerWave = 1.5f;
    [SerializeField] private float maxWaveBoost = 10f;

    private Camera cam;
    private float waveBoost;
    private float pulseAmount;
    private bool powerupActive;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFov;
    }

    void OnEnable()
    {
        GameManager.OnPowerupActivated += HandlePowerupOn;
        GameManager.OnPowerupDeactivated += HandlePowerupOff;
        GameManager.OnWaveChanged += HandleWaveChanged;
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameManager.OnPowerupActivated -= HandlePowerupOn;
        GameManager.OnPowerupDeactivated -= HandlePowerupOff;
        GameManager.OnWaveChanged -= HandleWaveChanged;
        GameManager.OnGameOver -= HandleGameOver;
    }

    void Update()
    {
        float targetPulse = powerupActive ? powerupFovBoost : 0f;
        pulseAmount = Mathf.Lerp(pulseAmount, targetPulse, Time.deltaTime * pulseSpeed);
        cam.fieldOfView = baseFov + waveBoost + pulseAmount;
    }

    private void HandlePowerupOn() => powerupActive = true;
    private void HandlePowerupOff() => powerupActive = false;

    private void HandleWaveChanged(int wave)
    {
        waveBoost = Mathf.Min((wave - 1) * fovPerWave, maxWaveBoost);
    }

    private void HandleGameOver()
    {
        powerupActive = false;
    }
}
