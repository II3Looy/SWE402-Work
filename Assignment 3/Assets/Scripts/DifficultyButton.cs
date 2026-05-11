using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private int difficulty;

    public void SetDifficulty()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame(difficulty);
    }
}
