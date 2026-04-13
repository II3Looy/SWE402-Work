using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

public class WallObject : CellObject
{
   [FormerlySerializedAs("ObstacleTile")]
   [SerializeField] private Tile m_ObstacleTile;
   [FormerlySerializedAs("MaxHealth")]
   [SerializeField, Range(1, 10)] private int m_MaxHealth = 3;

   private int m_HealthPoint;
   private Tile m_OriginalTile;
  
   public override void Init(Vector2Int cell)
   {
       base.Init(cell);

       m_HealthPoint = m_MaxHealth;
      
       m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
       GameManager.Instance.BoardManager.SetCellTile(cell, m_ObstacleTile);
   }

   public override bool PlayerWantsToEnter()
   {
       m_HealthPoint -= 1;
       GameManager.Instance.PlayWallAttackSfx();

       if (m_HealthPoint > 0)
       {
           return false;
       }

       GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
       GameManager.Instance.PlayWallDestroyVfx(transform.position);
       GameManager.Instance.BoardManager.RecycleObject(this);
       return true;
   }
}