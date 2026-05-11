using UnityEngine;

/// <summary>
/// One-shot pickup VFX. Drag a burst ParticleSystem prefab into the
/// <see cref="burstPrefab"/> slot on the GameObject that holds this script.
/// Static <see cref="PlayAt"/> spawns and plays the burst at a world position
/// and auto-destroys it after the system finishes.
/// </summary>
public class PickupVFX : MonoBehaviour
{
    public static PickupVFX Instance { get; private set; }

    [Tooltip("Particle System prefab played when the powerup is collected.")]
    [SerializeField] private ParticleSystem burstPrefab;

    [Tooltip("Seconds to wait before destroying the spawned burst.")]
    [SerializeField] private float lifetime = 2f;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play(Vector3 position)
    {
        if (burstPrefab == null) return;
        ParticleSystem instance = Instantiate(burstPrefab, position, Quaternion.identity);
        instance.Play();
        Destroy(instance.gameObject, lifetime);
    }

    /// <summary>Convenience static accessor; safe no-op if there is no PickupVFX in the scene.</summary>
    public static void PlayAt(Vector3 position)
    {
        if (Instance != null) Instance.Play(position);
    }
}
