using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : CellObject
{
    [Header("Stats")]
    [FormerlySerializedAs("Health")]
    [SerializeField, Range(1, 10)] private int m_Health = 3;
    [FormerlySerializedAs("FoodDamage")]
    [SerializeField, Range(1, 20)] private int m_FoodDamage = 3;

    private int m_CurrentHealth;
    private Animator m_Animator;
    private bool m_IsSubscribed;
    private bool m_HasAttackTrigger;
    private bool m_HasHurtTrigger;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CurrentHealth = m_Health;
        CacheAnimatorParams();
    }

    private void OnDisable()
    {
        if (m_IsSubscribed && GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
            m_IsSubscribed = false;
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = m_Health;
        TrySubscribeToTurns();
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;
        if (m_Animator != null && m_HasHurtTrigger) m_Animator.SetTrigger("Hurt");

        if (m_CurrentHealth <= 0)
        {
            GameManager.Instance.PlayEnemyDeathSfx();
            GameManager.Instance.PlayEnemyDeathVfx(transform.position);
            GameManager.Instance.BoardManager.RecycleObject(this);
        }

        return false;
    }

    void TurnHappened()
    {
        var playerController = GameManager.Instance.PlayerController;
        if (playerController == null) return;

        var playerCell = playerController.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
        {
            if (m_Animator != null && m_HasAttackTrigger) m_Animator.SetTrigger("Attack");
            GameManager.Instance.PlayEnemyAttackSfx();
            GameManager.Instance.ChangeFood(-m_FoodDamage);
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0) return MoveTo(m_Cell + Vector2Int.right);
        if (xDist < 0) return MoveTo(m_Cell + Vector2Int.left);
        return false;
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0) return MoveTo(m_Cell + Vector2Int.up);
        if (yDist < 0) return MoveTo(m_Cell + Vector2Int.down);
        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;

        transform.position = board.CellToWorld(coord);
        return true;
    }

    void TrySubscribeToTurns()
    {
        if (m_IsSubscribed || GameManager.Instance == null || GameManager.Instance.TurnManager == null)
        {
            return;
        }

        GameManager.Instance.TurnManager.OnTick += TurnHappened;
        m_IsSubscribed = true;
    }

    void CacheAnimatorParams()
    {
        if (m_Animator == null)
        {
            return;
        }

        AnimatorControllerParameter[] parameters = m_Animator.parameters;
        for (int i = 0; i < parameters.Length; ++i)
        {
            if (parameters[i].type != AnimatorControllerParameterType.Trigger)
            {
                continue;
            }

            if (parameters[i].name == "Attack")
            {
                m_HasAttackTrigger = true;
            }
            else if (parameters[i].name == "Hurt")
            {
                m_HasHurtTrigger = true;
            }
        }
    }
}