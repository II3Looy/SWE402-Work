using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("How fast this enemy chases the player")]
    [SerializeField, Range(1f, 10f)] private float speed = 3.0f;

    [Tooltip("Points awarded when this enemy is knocked off")]
    [SerializeField] private int pointValue = 10;

    [SerializeField] private ParticleSystem knockoutEffect;

    private Rigidbody enemyRb;
    private GameObject player;
    private bool isDestroyed = false;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");

        if (GameManager.Instance != null)
            speed = GameManager.Instance.enemySpeedModifier;
    }

    void Update()
    {
        if (isDestroyed || player == null) return;

        if (transform.position.y < -10)
        {
            isDestroyed = true;
            if (knockoutEffect != null)
            {
                ParticleSystem effect = Instantiate(knockoutEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 2f);
            }
            GameManager.RaiseEnemyKnockedOff(pointValue);
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance == null || !GameManager.Instance.isGameActive) return;

        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed);
    }
}
