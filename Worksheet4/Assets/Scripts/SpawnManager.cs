using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{   
    public GameObject[] warshipPrefabs;
    private float spawnInterval = 1.5f;
    private float spawnRangeX = 10;
    private float spawnPosZ = 20;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
        StartCoroutine(DifficultyRamp());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            int index = Random.Range(0, warshipPrefabs.Length);
            Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 3, spawnPosZ);
            
            Instantiate(warshipPrefabs[index], spawnPos, warshipPrefabs[index].transform.rotation);
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator DifficultyRamp()
    {
        // Wait for 30 seconds before the spike
        yield return new WaitForSeconds(30f);
        Debug.Log("Difficulty Increased!");

        while (spawnInterval > 0.3f)
        {
            spawnInterval -= 0.1f; // Decrease delay between spawns
            yield return new WaitForSeconds(5f); // Increase every 5s after the first 30s
        }
    }
}