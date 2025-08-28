using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Fighter", menuName = "Fighter SO/Base")]
public class SOFighterBase : SOFighterPayload
{
    [Header("基本ステータス")]
    [SerializeField] private int maxHealth = 100;
    public override int MaxHealth => maxHealth;

    [Header("移動関連")]
    [SerializeField] private float moveSpeed = 1f;
    public override float MoveSpeed => moveSpeed;

    [SerializeField] private float jumpForce = 1f;
    public override float JumpForce => jumpForce;

    [SerializeField] private float gravity = 1f;
    public override float Gravity => gravity;

    [Header("アニメーション")]
    [SerializeField] private RuntimeAnimatorController animatorOverrideController;
    public override RuntimeAnimatorController AnimatorController => animatorOverrideController;

    [Header("スキルで生成するもの")]
    [SerializeField] private HitboxPrefab[] hitboxPrefabs;
    public override HitboxPrefab[] HitboxPrefabs => hitboxPrefabs;

    [SerializeField] private EffectPrefab[] effectPrefabs;
    public override EffectPrefab[] EffectPrefabs => effectPrefabs;

    [SerializeField] private Action<int>[] triggerDelegates;
    public override Action<int>[] TriggerDelegates => triggerDelegates;
}
