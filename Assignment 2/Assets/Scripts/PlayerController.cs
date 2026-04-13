using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private Animator m_Animator;
    private Coroutine m_MoveCoroutine;

    private bool m_IsMoving;
    [FormerlySerializedAs("MoveSpeed")]
    [Tooltip("Grid travel speed in cells per second.")]
    [SerializeField, Range(1f, 20f)] private float m_MoveSpeed = 5f;
    public Vector2Int Cell => m_CellPosition;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            if (m_MoveCoroutine != null)
            {
                StopCoroutine(m_MoveCoroutine);
                m_MoveCoroutine = null;
            }
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            if (m_MoveCoroutine != null)
            {
                StopCoroutine(m_MoveCoroutine);
            }
            m_IsMoving = true;
            m_MoveCoroutine = StartCoroutine(MoveRoutine(m_Board.CellToWorld(m_CellPosition), m_CellPosition));
        }

        m_Animator.SetBool("Moving", m_IsMoving);
    }

    private void Update()
    {
        if (m_IsGameOver)
            return;

        if (m_IsMoving)
        {
            return;
        }

        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if(hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if(cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.ContainedObject == null)
                {
                    GameManager.Instance.PlayPlayerMoveSfx();
                    MoveTo(newCellTarget, false);
                }
                else if(cellData.ContainedObject.PlayerWantsToEnter())
                {
                    GameManager.Instance.PlayPlayerMoveSfx();
                    MoveTo(newCellTarget, false);
                }
            }
        }
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, Vector2Int destinationCell)
    {
        m_IsMoving = true;
        m_Animator.SetBool("Moving", true);
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / m_MoveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        m_IsMoving = false;
        m_MoveCoroutine = null;
        m_Animator.SetBool("Moving", false);

        BoardManager.CellData cellData = m_Board.GetCellData(destinationCell);
        if (cellData != null && cellData.ContainedObject != null)
        {
            cellData.ContainedObject.PlayerEntered();
        }
    }
}