using UnityEngine;
using UnityEngine.InputSystem;

public class Target : MonoBehaviour {
    private GameManager gameManager;
    [SerializeField] private float lifeTime = 3.0f;

    void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        float randomSize = Random.Range(0.5f, 1.5f);
        transform.localScale = Vector3.one * randomSize;
        Invoke("CheckMiss", lifeTime);
    }

    void Update() {
        // This is the "Brute Force" way for Mac Trackpads
        if (Input.GetMouseButtonDown(0) && gameManager.isGameActive) {
            // Create a ray from the mouse position
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // Check if the ray hit THIS object's collider
            if (hit.collider != null && hit.collider.gameObject == gameObject) {
                HitTarget();
            }
        }
    }

    void HitTarget() {
        CancelInvoke("CheckMiss");
        gameManager.UpdateScore(5);
        Destroy(gameObject);
    }

    void CheckMiss() {
        if (gameManager.isGameActive) {
            gameManager.GameOver();
        }
        Destroy(gameObject);
    }
}