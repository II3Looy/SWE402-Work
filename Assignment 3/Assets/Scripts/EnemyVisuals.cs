using System.Collections;
using UnityEngine;

/// <summary>
/// Runtime material control for enemies using MaterialPropertyBlock
/// (no shared-material instances, no asset edits).
///  - Tints the enemy redder as the wave number grows.
///  - Flashes white briefly when hit by the powered-up player.
/// Attach to the Enemy prefab.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class EnemyVisuals : MonoBehaviour
{
    [Tooltip("Original tint at wave 1.")]
    [SerializeField] private Color baseColor = new Color(0.9f, 0.9f, 0.9f);

    [Tooltip("Tint approached at maxWave or beyond.")]
    [SerializeField] private Color maxWaveColor = new Color(1f, 0.25f, 0.25f);

    [Tooltip("Wave at which the enemy reaches maxWaveColor.")]
    [SerializeField] private int maxWave = 8;

    [Tooltip("Flash color when hit.")]
    [SerializeField] private Color flashColor = Color.white;

    [Tooltip("Flash duration in seconds.")]
    [SerializeField] private float flashDuration = 0.12f;

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private Coroutine flashRoutine;
    private Color currentTint;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
        currentTint = baseColor;
        ApplyTint(currentTint);
    }

    void OnEnable()
    {
        GameManager.OnWaveChanged += HandleWaveChanged;
    }

    void OnDisable()
    {
        GameManager.OnWaveChanged -= HandleWaveChanged;
    }

    private void HandleWaveChanged(int wave)
    {
        float t = Mathf.Clamp01((wave - 1f) / Mathf.Max(1, maxWave - 1));
        currentTint = Color.Lerp(baseColor, maxWaveColor, t);
        if (flashRoutine == null) ApplyTint(currentTint);
    }

    /// <summary>Briefly flash to <see cref="flashColor"/> and return to current tint.</summary>
    public void Flash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        ApplyTint(flashColor);
        yield return new WaitForSeconds(flashDuration);
        ApplyTint(currentTint);
        flashRoutine = null;
    }

    private void ApplyTint(Color c)
    {
        if (rend == null) return;
        rend.GetPropertyBlock(mpb);
        mpb.SetColor(BaseColorId, c);
        mpb.SetColor(ColorId, c);
        rend.SetPropertyBlock(mpb);
    }
}
