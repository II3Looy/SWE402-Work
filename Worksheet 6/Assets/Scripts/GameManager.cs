using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using System.Collections;

public class GameManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI waveText; 
    [SerializeField] private GameObject titleScreen; 
    [SerializeField] private GameObject gameOverPanel; 
    
    [Header("Settings")]
    [Range(0.1f, 3f)] public float spawnRate; 
    [Tooltip("Controls if spawning and scoring are allowed")] public bool isGameActive; 

    private int score;
    private int wave = 1;

    void Start() {
        isGameActive = false; 
    }

    public void StartGame(int difficulty) {
        isGameActive = true; 
        score = 0;
        wave = 1;
        
        spawnRate = 2.0f / difficulty; 
        titleScreen.SetActive(false); 
        
        UpdateScore(0);
        UpdateWaveText();
    }

    public void UpdateScore(int pointsToAdd) {
        if (!isGameActive) return; 
        
        score += pointsToAdd;
        scoreText.text = "Score: " + score;

        if (score > 0 && score % 50 == 0) {
            wave++;
            UpdateWaveText();
            spawnRate *= 0.9f;
        }
    }

    private void UpdateWaveText() {
        waveText.text = "Wave: " + wave; 
    }

    public void GameOver() {
        isGameActive = false; 
        gameOverPanel.SetActive(true); 
        
        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg != null) {
            StartCoroutine(FadeIn(cg, 0.5f)); 
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    IEnumerator FadeIn(CanvasGroup cg, float dur) {
        float t = 0;
        while (t < dur) {
            t += Time.deltaTime;
            cg.alpha = t / dur; 
            yield return null; 
        }
        cg.alpha = 1;
    }
}