using UnityEngine;

/// <summary>
/// Singleton camera shake. Call CameraShake.Instance.Shake(duration, magnitude)
/// from anywhere. The script remembers the camera's local rest position and
/// returns to it after each shake.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Tooltip("Camera Transform to shake. If empty, this Transform is used.")]
    [SerializeField] private Transform target;

    [Tooltip("Default duration if a caller passes 0.")]
    [SerializeField] private float defaultDuration = 0.18f;

    [Tooltip("Default magnitude if a caller passes 0.")]
    [SerializeField] private float defaultMagnitude = 0.15f;

    private Vector3 restLocalPosition;
    private float shakeTimer;
    private float currentMagnitude;
    private float currentDuration;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (target == null) target = transform;
        restLocalPosition = target.localPosition;
    }

    void OnEnable()
    {
        GameManager.OnPlayerHitEnemy += HandlePlayerHit;
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameManager.OnPlayerHitEnemy -= HandlePlayerHit;
        GameManager.OnGameOver -= HandleGameOver;
    }

    void LateUpdate()
    {
        if (shakeTimer <= 0f)
        {
            target.localPosition = restLocalPosition;
            return;
        }

        shakeTimer -= Time.deltaTime;
        float falloff = Mathf.Clamp01(shakeTimer / currentDuration);
        Vector3 offset = Random.insideUnitSphere * (currentMagnitude * falloff);
        offset.z = 0f;
        target.localPosition = restLocalPosition + offset;
    }

    public void Shake(float duration, float magnitude)
    {
        currentDuration = duration > 0f ? duration : defaultDuration;
        currentMagnitude = magnitude > 0f ? magnitude : defaultMagnitude;
        shakeTimer = currentDuration;
    }

    private void HandlePlayerHit(Vector3 _)
    {
        Shake(0.18f, 0.2f);
    }

    private void HandleGameOver()
    {
        Shake(0.45f, 0.35f);
    }
}
