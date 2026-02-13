using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement variables from your original file
    public float horizontalInput;
    public float speed = 10.0f;
    public float xRange = 10;

    void Update()
    {
        // --- FIRING LOGIC (Object Pooled) ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Try to get the single bullet from the pool
            GameObject bullet = BulletPool.Instance.GetBullet();
            
            if (bullet != null)
            {
                // Move the pooled bullet to the player's position and activate it
                bullet.transform.position = transform.position + Vector3.forward * 1.5f;
                bullet.transform.rotation = transform.rotation; 
                bullet.SetActive(true); 
            }
            else 
            {
                // If the bullet is already active (on screen), this will fail
                Debug.Log("Bullet already in flight - wait for reload!");
            }
        }

        // --- MOVEMENT LOGIC (From your uploaded code) ---
        // Constraint: Keep player within the X range
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }

        // Standard horizontal movement
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
    }

    // --- COLLISION LOGIC ---
    private void OnTriggerEnter(Collider other)
    {
        // Per your requirement: Asteroid (Warship) hits player = Game Over
        if (other.CompareTag("Enemy")) 
        {
            Debug.Log("Game Over! The warship rammed you.");
            Time.timeScale = 0; // Freeze the game
        }
    }
}