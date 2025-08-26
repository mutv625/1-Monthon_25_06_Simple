using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Fighter", menuName = "Fighter SO/Base")]
public class SOFighterBase : SOFighterPayload
{
    [Header("基本ステータス")]
    [SerializeField] private int maxHealth = 100;
    public override int MaxHealth => maxHealth;

    [SerializeField] private int defense = 0;
    public override int Defense => defense;

    [Header("移動関連")]
    [SerializeField] private float moveSpeedMult = 1f;
    public override float MoveSpeedMult => moveSpeedMult;

    [SerializeField] private float jumpForceMult = 1f;
    public override float JumpForceMult => jumpForceMult;

    [SerializeField] private float gravityMult = 1f;
    public override float GravityMult => gravityMult;

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
