using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private PlayerCore playerCore;
    [SerializeField] private List<Collider2D> groundCheckPoints;

    [SerializeField] private BoolReactiveProperty isGrounded = new BoolReactiveProperty(false);

    public void Activate(FightingEntryPoint fightingEntryPoint, PlayerCore playerCore)
    {
        this.playerCore = playerCore;

        // 戦闘時間内での地面判定の更新
        fightingEntryPoint.updateInFighting
            .Subscribe(_ => UpdateGroundCheck())
            .AddTo(this);

        // * 1. false -> true の瞬間リセット
        isGrounded
            .DistinctUntilChanged()
            .Where(isGrounded => isGrounded)
            .Subscribe(_ => playerCore.ResetJumpCount())
            .AddTo(this);

        // * 2. 地面にいるかどうかを PlayerCore に伝える
        isGrounded
            .Where(isGrounded => !isGrounded)
            .ThrottleFirst(TimeSpan.FromSeconds(0.1))
            .Subscribe(_ =>
            {
                if (playerCore.jumpCount.Value == 0)
                {
                    playerCore.jumpCount.Value = 1;
                }
            }).AddTo(this);
    }

    private void UpdateGroundCheck()
    {
        foreach (Collider2D point in groundCheckPoints)
        {
            // 地面判定を行う
            if (point.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                isGrounded.Value = true;
                return;
            }
            else
            {
                isGrounded.Value = false;
            }
        }
    }
}
