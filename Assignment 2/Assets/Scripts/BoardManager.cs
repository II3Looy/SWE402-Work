using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    [Header("Prefabs")]
    [FormerlySerializedAs("FoodPrefab")]
    [SerializeField] private FoodObject m_FoodPrefab;
    [FormerlySerializedAs("WallPrefab")]
    [SerializeField] private WallObject m_WallPrefab;
    [FormerlySerializedAs("ExitCellPrefab")]
    [SerializeField] private ExitCellObject m_ExitCellPrefab;
    [FormerlySerializedAs("EnemyPrefab")]
    [SerializeField] private Enemy m_EnemyPrefab;

    [Header("Board")]
    [FormerlySerializedAs("Width")]
    [SerializeField, Range(8, 64)] private int m_Width = 16;
    [FormerlySerializedAs("Height")]
    [SerializeField, Range(8, 64)] private int m_Height = 16;
    [FormerlySerializedAs("GroundTiles")]
    [SerializeField] private Tile[] m_GroundTiles;
    [FormerlySerializedAs("WallTiles")]
    [SerializeField] private Tile[] m_WallTiles;

    [Header("Spawn Counts")]
    [SerializeField, Range(1, 30)] private int m_FoodCount = 5;
    [SerializeField, Range(1, 40)] private int m_MinWallCount = 6;
    [SerializeField, Range(1, 40)] private int m_MaxWallCount = 10;
    [SerializeField, Range(0, 20)] private int m_EnemyCount = 2;

    [Header("Pool Sizes")]
    [Tooltip("Pre-instantiated food objects available for reuse.")]
    [SerializeField, Range(1, 64)] private int m_FoodPoolSize = 16;
    [Tooltip("Pre-instantiated wall objects available for reuse.")]
    [SerializeField, Range(1, 64)] private int m_WallPoolSize = 24;
    [Tooltip("Pre-instantiated enemy objects available for reuse.")]
    [SerializeField, Range(1, 32)] private int m_EnemyPoolSize = 8;

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    private ExitCellObject m_ExitInstance;
    private List<Vector2Int> m_EmptyCellsList;
    private readonly List<FoodObject> m_FoodPool = new();
    private readonly List<WallObject> m_WallPool = new();
    private readonly List<Enemy> m_EnemyPool = new();

    public int Width => m_Width;
    public int Height => m_Height;

    private void Awake()
    {
        BuildPools();
    }

    public void Init()
    {
        if (!HasValidConfig())
        {
            return;
        }

        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_EmptyCellsList = new List<Vector2Int>();
        m_BoardData = new CellData[m_Width, m_Height];

        for (int y = 0; y < m_Height; ++y)
        {
            for (int x = 0; x < m_Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == m_Width - 1 || y == m_Height - 1)
                {
                    tile = GetRandomTile(m_WallTiles);
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GetRandomTile(m_GroundTiles);
                    m_BoardData[x, y].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
        Vector2Int endCoord = new Vector2Int(m_Width - 2, m_Height - 2);
        if (m_ExitInstance == null)
        {
            m_ExitInstance = Instantiate(m_ExitCellPrefab, transform);
        }
        m_ExitInstance.gameObject.SetActive(true);
        AddObject(m_ExitInstance, endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateWall();
        GenerateFood();
        GenerateEnemies();
    }

    public void RecycleObject(CellObject obj)
    {
        if (obj == null)
        {
            return;
        }

        CellData cell = GetCellData(obj.Cell);
        if (cell != null && cell.ContainedObject == obj)
        {
            cell.ContainedObject = null;
        }

        obj.gameObject.SetActive(false);
    }

    void GenerateEnemies()
    {
        for (int i = 0; i < m_EnemyCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;

            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            Enemy newEnemy = GetPooledObject(m_EnemyPool, m_EnemyPrefab);
            AddObject(newEnemy, coord);
        }
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= m_Width
            || cellIndex.y < 0 || cellIndex.y >= m_Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    void GenerateWall()
    {
        int maxWalls = Mathf.Max(m_MinWallCount + 1, m_MaxWallCount + 1);
        int wallCount = Random.Range(m_MinWallCount, maxWalls);
        for (int i = 0; i < wallCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = GetPooledObject(m_WallPool, m_WallPrefab);
            AddObject(newWall, coord);
        }
    }

    void GenerateFood()
    {
        for (int i = 0; i < m_FoodCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = GetPooledObject(m_FoodPool, m_FoodPrefab);
            AddObject(newFood, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void Clean()
    {
        if(m_BoardData == null) return;

        for (int y = 0; y < m_Height; ++y)
        {
            for (int x = 0; x < m_Width; ++x)
            {
                var cellData = m_BoardData[x, y];
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.gameObject.SetActive(false);
                    cellData.ContainedObject = null;
                }
                SetCellTile(new Vector2Int(x,y), null);
            }
        }
    }

    void BuildPools()
    {
        if (!HasValidConfig())
        {
            return;
        }

        CreatePool(m_FoodPrefab, m_FoodPoolSize, m_FoodPool);
        CreatePool(m_WallPrefab, m_WallPoolSize, m_WallPool);
        CreatePool(m_EnemyPrefab, m_EnemyPoolSize, m_EnemyPool);
    }

    void CreatePool<T>(T prefab, int size, List<T> pool) where T : CellObject
    {
        for (int i = 0; i < size; ++i)
        {
            T instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            pool.Add(instance);
        }
    }

    T GetPooledObject<T>(List<T> pool, T prefab) where T : CellObject
    {
        for (int i = 0; i < pool.Count; ++i)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }

        T instance = Instantiate(prefab, transform);
        pool.Add(instance);
        return instance;
    }

    Tile GetRandomTile(Tile[] tiles)
    {
        return tiles[Random.Range(0, tiles.Length)];
    }

    bool HasValidConfig()
    {
        if (m_FoodPrefab == null || m_WallPrefab == null || m_ExitCellPrefab == null || m_EnemyPrefab == null)
        {
            Debug.LogError("BoardManager is missing one or more required prefabs.", this);
            return false;
        }

        if (m_GroundTiles == null || m_GroundTiles.Length == 0 || m_WallTiles == null || m_WallTiles.Length == 0)
        {
            Debug.LogError("BoardManager needs at least one ground tile and one wall tile.", this);
            return false;
        }

        return true;
    }
}