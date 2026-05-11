using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private Button restartButton;

    void Start()
    {
        gameOverPanel.SetActive(false);

        if (gameOverCanvasGroup == null)
            gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void OnEnable()
    {
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= HandleGameOver;
    }

    void HandleGameOver()
    {
        gameOverPanel.SetActive(true);

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 0;
            StartCoroutine(FadeIn(gameOverCanvasGroup, 0.5f));
        }
    }

    IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }
        cg.alpha = 1;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
