using UnityEngine;

/// <summary>
/// Controls a ParticleSystem trail / loop on the player.
///  - Plays while the player is moving and the game is active.
///  - Stops automatically when idle or on game over.
/// Attach to the Player GameObject and drag in the trail ParticleSystem.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerTrailVFX : MonoBehaviour
{
    [Tooltip("Trail ParticleSystem (set Looping = true on the system).")]
    [SerializeField] private ParticleSystem trail;

    [Tooltip("Speed above which the trail should be playing.")]
    [SerializeField] private float speedThreshold = 1.5f;

    private Rigidbody rb;
    private bool isPlaying;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= HandleGameOver;
    }

    void Update()
    {
        if (trail == null) return;

        bool active = GameManager.Instance != null && GameManager.Instance.isGameActive;
        bool shouldPlay = active && rb.linearVelocity.sqrMagnitude > speedThreshold * speedThreshold;

        if (shouldPlay && !isPlaying)
        {
            trail.Play();
            isPlaying = true;
        }
        else if (!shouldPlay && isPlaying)
        {
            trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            isPlaying = false;
        }
    }

    private void HandleGameOver()
    {
        if (trail != null) trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        isPlaying = false;
    }
}
