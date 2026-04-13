using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [FormerlySerializedAs("BoardManager")]
    [SerializeField] private BoardManager m_BoardManager;
    [FormerlySerializedAs("PlayerController")]
    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField, Range(1, 200)] private int m_StartingFood = 20;

    public TurnManager TurnManager { get; private set; }
    private int m_FoodAmount = 100;

    [FormerlySerializedAs("UIDoc")]
    [SerializeField] private UIDocument m_UIDoc;
    [Header("Audio")]
    [SerializeField] private AudioClip m_PlayerMoveSfx;
    [SerializeField] private AudioClip m_WallAttackSfx;
    [SerializeField] private AudioClip m_FoodPickupSfx;
    [SerializeField] private AudioClip m_EnemyAttackSfx;
    [SerializeField] private AudioClip m_EnemyDeathSfx;
    [SerializeField] private AudioClip m_GameOverSfx;
    [SerializeField, Range(0f, 1f)] private float m_DefaultSfxVolume = 1f;

    [Header("VFX")]
    [Tooltip("Played when a wall is fully destroyed.")]
    [SerializeField] private ParticleSystem m_WallDestroyVfxPrefab;
    [Tooltip("Played when food is collected.")]
    [SerializeField] private ParticleSystem m_FoodCollectVfxPrefab;
    [Tooltip("Played when an enemy dies.")]
    [SerializeField] private ParticleSystem m_EnemyDeathVfxPrefab;
    [SerializeField, Range(0, 20)] private int m_WallDestroyVfxPoolSize = 6;
    [SerializeField, Range(0, 20)] private int m_FoodCollectVfxPoolSize = 8;
    [SerializeField, Range(0, 20)] private int m_EnemyDeathVfxPoolSize = 6;

    private Label m_FoodLabel;
    private int m_CurrentLevel = 1;
    private bool m_HasGameOverTriggered;
    private AudioSource m_SfxSource;
    private readonly List<ParticleSystem> m_WallDestroyVfxPool = new();
    private readonly List<ParticleSystem> m_FoodCollectVfxPool = new();
    private readonly List<ParticleSystem> m_EnemyDeathVfxPool = new();

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
        EnsureSfxSource();
        BuildVfxPools();
    }

    void Start()
    {
        TryInitializeUI();

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
        if (m_FoodLabel != null)
        {
            m_FoodLabel.text = "Food : " + m_FoodAmount;
        }
        if (m_FoodAmount <= 0)
        {
            if (!m_HasGameOverTriggered)
            {
                PlaySfx(m_GameOverSfx);
                m_HasGameOverTriggered = true;
            }

            if (PlayerController != null)
            {
                PlayerController.GameOver();
            }

            if (m_GameOverPanel != null)
            {
                m_GameOverPanel.style.visibility = Visibility.Visible;
            }

            if (m_GameOverMessage != null)
            {
                m_GameOverMessage.text = "You starved to death at level " + m_CurrentLevel;
            }
        }
    }

    public void NewLevel()
    {
        m_BoardManager.Clean();
        m_BoardManager.Init();
        m_PlayerController.Spawn(m_BoardManager, new Vector2Int(1, 1));
        m_CurrentLevel++;
    }
    public void StartNewGame()
    {
        TryInitializeUI();
        m_FoodAmount = m_StartingFood;
        m_HasGameOverTriggered = false;
        if (m_FoodLabel != null)
        {
            m_FoodLabel.text = "Food : " + m_FoodAmount;
        }
        m_CurrentLevel = 1;
        m_BoardManager.Clean();
        m_BoardManager.Init();
        m_PlayerController.Spawn(m_BoardManager, new Vector2Int(1, 1));
        if (m_GameOverPanel != null)
        {
            m_GameOverPanel.style.visibility = Visibility.Hidden;
        }
    }

    public BoardManager BoardManager => m_BoardManager;
    public PlayerController PlayerController => m_PlayerController;
    public void PlayPlayerMoveSfx() => PlaySfx(m_PlayerMoveSfx);
    public void PlayWallAttackSfx() => PlaySfx(m_WallAttackSfx);
    public void PlayFoodPickupSfx() => PlaySfx(m_FoodPickupSfx);
    public void PlayEnemyAttackSfx() => PlaySfx(m_EnemyAttackSfx);
    public void PlayEnemyDeathSfx() => PlaySfx(m_EnemyDeathSfx);
    public void PlayWallDestroyVfx(Vector3 worldPosition) => PlayPooledVfx(m_WallDestroyVfxPool, m_WallDestroyVfxPrefab, worldPosition);
    public void PlayFoodCollectVfx(Vector3 worldPosition) => PlayPooledVfx(m_FoodCollectVfxPool, m_FoodCollectVfxPrefab, worldPosition);
    public void PlayEnemyDeathVfx(Vector3 worldPosition) => PlayPooledVfx(m_EnemyDeathVfxPool, m_EnemyDeathVfxPrefab, worldPosition);

    void TryInitializeUI()
    {
        if (m_UIDoc == null)
        {
            return;
        }

        VisualElement root = m_UIDoc.rootVisualElement;
        if (root == null)
        {
            return;
        }

        m_GameOverPanel = root.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel != null ? m_GameOverPanel.Q<Label>("GameOverMessage") : null;
        m_FoodLabel = root.Q<Label>("FoodLabel");
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        EnsureSfxSource();

        if (m_SfxSource == null)
        {
            return;
        }

        m_SfxSource.PlayOneShot(clip, m_DefaultSfxVolume);
    }

    void EnsureSfxSource()
    {
        if (m_SfxSource != null)
        {
            return;
        }

        m_SfxSource = GetComponent<AudioSource>();
        if (m_SfxSource == null)
        {
            m_SfxSource = gameObject.AddComponent<AudioSource>();
            m_SfxSource.playOnAwake = false;
            m_SfxSource.spatialBlend = 0f;
        }
    }

    void BuildVfxPools()
    {
        BuildVfxPool(m_WallDestroyVfxPrefab, m_WallDestroyVfxPoolSize, m_WallDestroyVfxPool);
        BuildVfxPool(m_FoodCollectVfxPrefab, m_FoodCollectVfxPoolSize, m_FoodCollectVfxPool);
        BuildVfxPool(m_EnemyDeathVfxPrefab, m_EnemyDeathVfxPoolSize, m_EnemyDeathVfxPool);
    }

    void BuildVfxPool(ParticleSystem prefab, int size, List<ParticleSystem> pool)
    {
        if (prefab == null || size <= 0)
        {
            return;
        }

        for (int i = 0; i < size; ++i)
        {
            ParticleSystem vfx = Instantiate(prefab, transform);
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            vfx.gameObject.SetActive(false);
            pool.Add(vfx);
        }
    }

    void PlayPooledVfx(List<ParticleSystem> pool, ParticleSystem prefab, Vector3 worldPosition)
    {
        if (prefab == null)
        {
            return;
        }

        ParticleSystem vfx = GetPooledVfx(pool, prefab);
        vfx.transform.position = worldPosition;
        vfx.gameObject.SetActive(true);
        vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        vfx.Play();
        StartCoroutine(StopAndRecycleVfx(vfx));
    }

    ParticleSystem GetPooledVfx(List<ParticleSystem> pool, ParticleSystem prefab)
    {
        for (int i = 0; i < pool.Count; ++i)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                return pool[i];
            }
        }

        ParticleSystem vfx = Instantiate(prefab, transform);
        vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        vfx.gameObject.SetActive(false);
        pool.Add(vfx);
        return vfx;
    }

    IEnumerator StopAndRecycleVfx(ParticleSystem vfx)
    {
        if (vfx == null)
        {
            yield break;
        }

        while (vfx.IsAlive(true))
        {
            yield return null;
        }

        vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        vfx.gameObject.SetActive(false);
    }
}
