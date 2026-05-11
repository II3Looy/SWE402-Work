using UnityEngine;

/// <summary>
/// Gentle random flicker for a Light, useful for arena edge lights or
/// powerup spawn beacons. Drives intensity within a min/max range using
/// Perlin noise for a natural look.
/// Attach to any GameObject with a Light component.
/// </summary>
[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.6f;
    [Tooltip("How fast the noise advances.")]
    [SerializeField] private float speed = 4f;

    private Light targetLight;
    private float seed;

    void Awake()
    {
        targetLight = GetComponent<Light>();
        seed = Random.value * 100f;
    }

    void Update()
    {
        float n = Mathf.PerlinNoise(seed + Time.time * speed, 0f);
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, n);
    }
}
