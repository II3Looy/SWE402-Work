using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnGameOver;
    public static event Action<int> OnEnemyKnockedOff;
    public static event Action OnGameStarted;
    public static event Action<int> OnWaveChanged;
    public static event Action OnPowerupActivated;
    public static event Action OnPowerupDeactivated;
    public static event Action<Vector3> OnPlayerHitEnemy;

    [HideInInspector] public bool isGameActive = false;
    [HideInInspector] public float enemySpeedModifier = 3f;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private CanvasGroup titleScreenCanvasGroup;
    [SerializeField] private SpawnManager spawnManager;

    private int score = 0;
    private GameObject player;
    private Coroutine scorePunchCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (isGameActive && player != null && player.transform.position.y < -2)
        {
            GameOver();
        }
    }

    public void StartGame(int difficulty)
    {
        switch (difficulty)
        {
            case 1: // Easy
                enemySpeedModifier = 2f;
                break;
            case 2: // Medium
                enemySpeedModifier = 3f;
                break;
            case 3: // Hard
                enemySpeedModifier = 5f;
                break;
        }

        isGameActive = true;

        score = 0;
        scoreText.text = "Score: 0";
        waveText.text = "Wave: 1";

        StartCoroutine(FadeOutTitleScreen(0.3f));
        OnGameStarted?.Invoke();
        OnWaveChanged?.Invoke(1);
    }

    IEnumerator FadeOutTitleScreen(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            titleScreenCanvasGroup.alpha = 1 - (t / duration);
            yield return null;
        }
        titleScreenCanvasGroup.alpha = 0;
        titleScreen.SetActive(false);
        spawnManager.SpawnEnemyWave(1);
    }

    public void UpdateScore(int points)
    {
        if (!isGameActive) return;
        score += points;
        scoreText.text = "Score: " + score;

        if (scorePunchCoroutine != null)
            StopCoroutine(scorePunchCoroutine);
        scorePunchCoroutine = StartCoroutine(ScorePunch(scoreText.transform, 0.2f));
    }

    IEnumerator ScorePunch(Transform target, float duration)
    {
        Vector3 original = Vector3.one;
        Vector3 punched = original * 1.3f;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(punched, original, t / duration);
            yield return null;
        }
        target.localScale = original;
        scorePunchCoroutine = null;
    }

    public void UpdateWaveText(int waveNumber)
    {
        waveText.text = "Wave: " + waveNumber;
        OnWaveChanged?.Invoke(waveNumber);
    }

    public void GameOver()
    {
        isGameActive = false;
        OnGameOver?.Invoke();
    }

    public static void RaiseEnemyKnockedOff(int points)
    {
        OnEnemyKnockedOff?.Invoke(points);
    }

    public static void RaisePowerupActivated()
    {
        OnPowerupActivated?.Invoke();
    }

    public static void RaisePowerupDeactivated()
    {
        OnPowerupDeactivated?.Invoke();
    }

    public static void RaisePlayerHitEnemy(Vector3 contactPoint)
    {
        OnPlayerHitEnemy?.Invoke(contactPoint);
    }
}
