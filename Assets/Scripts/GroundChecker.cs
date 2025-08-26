using UnityEngine;
using System.Collections.Generic;

public class GroundChecker : MonoBehaviour
{
    private FightingEntryPoint fightingEP;

    [SerializeField] private PlayerCore playerCore;

    [SerializeField] private List<Collider2D> groundCheckPoints;

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        fightingEP = fightingEntryPoint;
    }

    private void UpdateGroundCheck()
    {
        foreach (Collider2D point in groundCheckPoints)
        {
            // 地面判定を行う
            if (point.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                // TODO: 地面に接触している場合の処理
            }
        }
    }
}
