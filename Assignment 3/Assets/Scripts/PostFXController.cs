using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Gameplay-driven URP post-processing.
///  - Bloom and ChromaticAberration intensify while powered up.
///  - Vignette intensifies on game over (and pulses slightly while powered up).
/// Attach to your Global Volume GameObject (the one with the Volume component).
/// In the Volume's profile, make sure Bloom, Vignette, and ChromaticAberration
/// overrides are added and their checkboxes enabled so the values can be driven.
/// </summary>
[RequireComponent(typeof(Volume))]
public class PostFXController : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField, Range(0f, 3f)] private float baseBloom = 0.6f;
    [SerializeField, Range(0f, 3f)] private float powerupBloom = 1.6f;

    [SerializeField, Range(0f, 1f)] private float baseChromatic = 0.05f;
    [SerializeField, Range(0f, 1f)] private float powerupChromatic = 0.6f;

    [SerializeField, Range(0f, 1f)] private float baseVignette = 0.2f;
    [SerializeField, Range(0f, 1f)] private float powerupVignette = 0.32f;
    [SerializeField, Range(0f, 1f)] private float gameOverVignette = 0.55f;

    [Tooltip("Lerp speed for transitions.")]
    [SerializeField] private float lerpSpeed = 4f;

    private Volume volume;
    private Bloom bloom;
    private Vignette vignette;
    private ChromaticAberration chromatic;

    private bool powerupActive;
    private bool gameOver;

    void Awake()
    {
        volume = GetComponent<Volume>();
        if (volume.profile != null)
        {
            volume.profile.TryGet(out bloom);
            volume.profile.TryGet(out vignette);
            volume.profile.TryGet(out chromatic);
        }
    }

    void OnEnable()
    {
        GameManager.OnPowerupActivated += HandlePowerupOn;
        GameManager.OnPowerupDeactivated += HandlePowerupOff;
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameStarted += HandleGameStarted;
    }

    void OnDisable()
    {
        GameManager.OnPowerupActivated -= HandlePowerupOn;
        GameManager.OnPowerupDeactivated -= HandlePowerupOff;
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameStarted -= HandleGameStarted;
    }

    void Update()
    {
        float targetBloom = powerupActive ? powerupBloom : baseBloom;

        float targetChrom = powerupActive ? powerupChromatic : baseChromatic;

        float targetVig;
        if (gameOver) targetVig = gameOverVignette;
        else if (powerupActive) targetVig = powerupVignette;
        else targetVig = baseVignette;

        if (bloom != null)
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, targetBloom, Time.deltaTime * lerpSpeed);

        if (chromatic != null)
            chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, targetChrom, Time.deltaTime * lerpSpeed);

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVig, Time.deltaTime * lerpSpeed);
    }

    private void HandlePowerupOn() => powerupActive = true;
    private void HandlePowerupOff() => powerupActive = false;
    private void HandleGameOver() => gameOver = true;
    private void HandleGameStarted()
    {
        gameOver = false;
        powerupActive = false;
    }
}
