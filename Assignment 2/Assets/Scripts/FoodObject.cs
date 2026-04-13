using UnityEngine;
using UnityEngine.Serialization;
public class FoodObject : CellObject
{
   [FormerlySerializedAs("AmountGranted")]
   [SerializeField, Range(1, 50)] private int m_AmountGranted = 10;

   public override void PlayerEntered()
   {
       GameManager.Instance.PlayFoodPickupSfx();
       GameManager.Instance.PlayFoodCollectVfx(transform.position);
       GameManager.Instance.ChangeFood(m_AmountGranted);
       GameManager.Instance.BoardManager.RecycleObject(this);
   }
}