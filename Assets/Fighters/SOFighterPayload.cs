using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SOFighterPayload : ScriptableObject
{
    [Header("基本ステータス")]
    public abstract int MaxHealth { get; }

    public abstract float MoveSpeed { get; }
    public abstract float JumpForce { get; }
    public abstract float Gravity { get; }

    /// <summary>
    /// アニメーターのコントローラー<br/>
    /// スキルやエフェクトはAnimatorOverrideControllerで設定する
    /// </summary>
    public abstract RuntimeAnimatorController AnimatorController { get; }

    public abstract HitboxPrefab[] HitboxPrefabs { get; }
    public abstract EffectPrefab[] EffectPrefabs { get; }
    public abstract Action<int>[] TriggerDelegates { get; }
}
