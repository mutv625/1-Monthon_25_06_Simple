using UnityEngine;
using UniRx;

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
        playerCore.IsDashing
            .Subscribe(isDashing => AnimateDash(isDashing));
    }

    private void AnimateDash(bool isDashing)
    {
        animator.SetBool("isDashing", isDashing);
    }
}