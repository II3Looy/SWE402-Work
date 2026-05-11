using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;

    [Tooltip("Player movement speed multiplier")]
    [SerializeField, Range(1f, 20f)] private float speed = 5.0f;

    private bool hasPowerup;
    [SerializeField] private float powerupStrength = 15.0f;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private AudioClip powerupClip;
    private AudioSource audioSource;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive) return;

        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed);
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive) return;

        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Vector3 pickupPos = other.transform.position;
            Destroy(other.gameObject);
            audioSource.PlayOneShot(powerupClip, 1.0f);
            GameManager.RaisePowerupActivated();
            PickupVFX.PlayAt(pickupPos);
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        powerupIndicator.gameObject.SetActive(false);
        GameManager.RaisePowerupDeactivated();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.Instance == null || !GameManager.Instance.isGameActive) return;

        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position).normalized;
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            Vector3 contactPoint = collision.GetContact(0).point;
            GameManager.RaisePlayerHitEnemy(contactPoint);

            EnemyVisuals visuals = collision.gameObject.GetComponent<EnemyVisuals>();
            if (visuals != null) visuals.Flash();
        }
    }
}
