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

    private AnimatorStateInfo animatorStateInfo;
    private BoolReactiveProperty isIdling = new BoolReactiveProperty(true);

    public void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        playerCore = GetComponent<PlayerCore>();

        fightingEntryPoint.updateInFighting
            .Subscribe(_ =>
            {
                animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                isIdling.Value = animatorStateInfo.IsName("Idling");
            });

        isIdling
            .DistinctUntilChanged()
            .Where(isIdling => isIdling)
            .Subscribe(_ => playerCore.ResetLockingStatusAtCore());

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
            .Where(state => state != AttackingStates.None)
            .Subscribe(state => AnimateSkill(state));
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
        if (jumpCount <= 0)
        {
            // 何もしない?
        }
        else if (jumpCount <= 2)
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
    private void AnimateSkill(AttackingStates state)
    {
        switch (state)
        {
            case AttackingStates.None:
                break;
            case AttackingStates.SkillAx:
                animator.SetTrigger("trigSkillAx");
                break;
            case AttackingStates.SkillBx:
                animator.SetTrigger("trigSkillBx");
                break;
            case AttackingStates.SkillA:
                animator.SetTrigger("trigSkillA");
                break;
            case AttackingStates.SkillB:
                animator.SetTrigger("trigSkillB");
                break;
            default:
                break;
        }
    }
}