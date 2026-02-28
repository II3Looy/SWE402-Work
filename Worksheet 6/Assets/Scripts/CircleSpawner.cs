using UnityEngine;
using System.Collections;

public class CircleSpawner : MonoBehaviour {
    [SerializeField] private GameObject targetPrefab; 
    private GameManager gameManager;

    void Start() {
        // Caching the reference - Requirement: No GameObject.Find in Update
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); 
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine() {
        while (true) {
            if (gameManager.isGameActive) {
                yield return new WaitForSeconds(gameManager.spawnRate);
                // Spawns within typical 2D screen bounds
                Vector2 spawnPos = new Vector2(Random.Range(-7, 7), Random.Range(-4, 4));
                Instantiate(targetPrefab, spawnPos, Quaternion.identity);
            }
            else {
                yield return null; 
            }
        }
    }
}