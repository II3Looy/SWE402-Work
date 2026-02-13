using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    private float topBound = 30;
    private float lowerBound = -3;

    void Update()
    {
        // If bullet (this object) goes out of bounds, deactivate it
        if (transform.position.z > topBound)
        {
            gameObject.SetActive(false);
        }
        // If warship/enemy reaches the lower bound
        else if (transform.position.z < lowerBound) 
        {
            Debug.Log("Game Over!");
            Time.timeScale = 0; // Freeze the game
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Enemy")) 
        {
            Destroy(other.gameObject); // Destroy the warship
            gameObject.SetActive(false); // RETURN bullet to pool
        }
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Game Over!");
            Time.timeScale = 0;
        }
    }
}