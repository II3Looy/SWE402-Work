using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour {
    private Button button;
    private GameManager gameManager;
    
    [SerializeField] [Tooltip("Set this to 1, 2, or 3 in the Inspector")]
    private int difficulty; 

    void Start() {
        button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Requirement: OnClick calls StartGame via SetDifficulty
        button.onClick.AddListener(SetDifficulty); 
    }

    void SetDifficulty() {
        gameManager.StartGame(difficulty); 
    }
}