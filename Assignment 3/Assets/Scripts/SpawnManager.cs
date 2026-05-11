using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [Tooltip("Number of units from center where enemies can spawn")]
    [SerializeField] private float spawnRange = 9.0f;

    [SerializeField] private GameObject powerupPrefab;

    private int enemyCount;
    private int waveNumber = 1;

    void OnEnable()
    {
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnEnemyKnockedOff += OnEnemyDestroyed;
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnEnemyKnockedOff -= OnEnemyDestroyed;
    }

    void HandleGameOver()
    {
        // isGameActive is already false — spawning gated in OnEnemyDestroyed
    }

    void OnEnemyDestroyed(int points)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UpdateScore(points);

        enemyCount--;
        if (enemyCount <= 0 && GameManager.Instance != null && GameManager.Instance.isGameActive)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);

            if (GameManager.Instance != null)
                GameManager.Instance.UpdateWaveText(waveNumber);
        }
    }

    public void SpawnEnemyWave(int enemiesToSpawn)
    {
        enemyCount = enemiesToSpawn;
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
        SpawnPowerup();
    }

    private void SpawnPowerup()
    {
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }
}
