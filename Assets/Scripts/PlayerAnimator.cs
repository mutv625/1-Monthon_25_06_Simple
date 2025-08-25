using UnityEngine;
using UniRx;

/// <summary>
/// ! 条件設定は主にこっち (AnimatorController はあくまで保険)
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    /// <summary>
    /// プレイヤーのアニメーション遷移を管理する。<br/>
    /// AnimationOverrideControllerをいれることもできる。
    /// </summary>
    [SerializeField] private Animator animator;

    [SerializeField] private PlayerCore playerCore;

    public void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        playerCore = GetComponent<PlayerCore>();


        playerCore.isDashing
            .DistinctUntilChanged()
            .Where(isDashing => isDashing)
            .Subscribe(_ => AnimateStartDashing());

        playerCore.isDashing
            .Subscribe(isDashing => AnimateDashing(isDashing));

        playerCore.jumpCount
            .DistinctUntilChanged()
            .Subscribe(jumpCount => AnimateJump(jumpCount));
    }

    private void AnimateStartDashing()
    {
        animator.SetTrigger("trigDashing");
    }
    private void AnimateDashing(bool isDashing)
    {
        animator.SetBool("isDashing", isDashing);
    }

    private void AnimateJump(int jumpCount)
    {
        if (jumpCount <= 2)
        {
            animator.SetTrigger($"trigJump{jumpCount}");
        }
        else
        {
            animator.SetTrigger("trigJump2");
        }

        animator.SetInteger("jumpCount", jumpCount);
    }
}