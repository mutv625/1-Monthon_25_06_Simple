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

        // ! 条件付きでアニメーションを切り替える
        // * 移動系
        playerCore.isDashing
            .DistinctUntilChanged()
            .Where(isDashing => isDashing)
            .Subscribe(_ => AnimateStartDashing());

        playerCore.isDashing
            .Subscribe(isDashing => AnimateDashing(isDashing));

        playerCore.jumpCount
            .DistinctUntilChanged()
            .Subscribe(jumpCount => AnimateJump(jumpCount));


        // * スキル系
        // TODO: AttackingStatesを参照するようにする

        playerCore.onSkill
            .Where(state => state == AttackingStates.SkillA)
            .Subscribe(_ => AnimateSkillA());

        playerCore.onSkill
            .Where(state => state == AttackingStates.SkillB)
            .Subscribe(_ => AnimateSkillB());
    }

    // ! アニメーション制御
    // * 移動系
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

    // * スキル系
    private void AnimateSkillA()
    {
        animator.SetTrigger("trigSkillA");
    }

    private void AnimateSkillB()
    {
        animator.SetTrigger("trigSkillB");
    }
}