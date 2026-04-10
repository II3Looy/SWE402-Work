using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;

    public TurnManager TurnManager { get; private set; }
    private int m_FoodAmount = 100;

    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private int m_CurrentLevel = 1;

    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;
        StartNewGame();
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
             m_GameOverMessage.text = "You starved to death at level " + m_CurrentLevel;
        }
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        m_CurrentLevel++;
    }
    public void StartNewGame()
    {
        m_FoodAmount = 20;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
        m_CurrentLevel = 1;
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        m_GameOverPanel.style.visibility = Visibility.Hidden;
    }
}
