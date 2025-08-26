using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class SOFighterPayload : ScriptableObject
{
    [Header("基本ステータス")]
    public abstract int MaxHealth { get; }
    public abstract int Defense { get; }

    public abstract float MoveSpeedMult { get; }
    public abstract float JumpForceMult { get; }
    public abstract float GravityMult { get; }

    /// <summary>
    /// アニメーターのコントローラー<br/>
    /// スキルやエフェクトはAnimatorOverrideControllerで設定する
    /// </summary>
    public abstract RuntimeAnimatorController AnimatorController { get; }

    public abstract HitboxPrefab[] HitboxPrefabs { get; }
    public abstract EffectPrefab[] EffectPrefabs { get; }
    public abstract Action<int>[] TriggerDelegates { get; }
}
