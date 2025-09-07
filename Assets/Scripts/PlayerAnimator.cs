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
            }).AddTo(this);

        isIdling
            .DistinctUntilChanged()
            .Where(isIdling => isIdling)
            .Subscribe(_ => playerCore.ResetLockingStatusAtCore())
            .AddTo(this);

        // ! 条件付きでアニメーションを切り替える
        // * 移動系
        playerCore.isDashing
            .DistinctUntilChanged()
            .Where(isDashing => isDashing)
            .Subscribe(_ => AnimateStartDashing())
            .AddTo(this);

        playerCore.isDashing
            .Subscribe(isDashing => AnimateDashing(isDashing))
            .AddTo(this);

        playerCore.onJump
            .Subscribe(jumpStatus => AnimateJump(jumpStatus.Item2))
            .AddTo(this);

        playerCore.jumpCount
            .Subscribe(jumpCount => animator.SetInteger("jumpCount", jumpCount))
            .AddTo(this);


        // * スキル系
        playerCore.onSkill
            .Where(state => state.Item1 != AttackingStates.None)
            .Subscribe(state => AnimateSkill(state.Item1))
            .AddTo(this);


        playerCore.isHurting
            .DistinctUntilChanged()
            .Where(isHurting => isHurting)
            .Subscribe(_ => animator.SetTrigger("trigHurt1"))
            .AddTo(this);

        playerCore.comboState
            .Subscribe(state => {
                if (state == ComboStates.Trapped)
                {
                    animator.SetBool("isTrapped", true);
                }
                else
                {
                    animator.SetBool("isTrapped", false);
                }
            })
            .AddTo(this);

        playerCore.comboState
            .Subscribe(state =>
            {
                if (state == ComboStates.Trapped)
                {
                    animator.speed = 0f;
                }
                else
                {
                    animator.speed = 1f;
                }
            }).AddTo(this);
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