using UnityEngine;

public abstract class SOFighterPayload : ScriptableObject
{
    [Header("基本ステータス")]
    public abstract int MaxHealth { get; }
    public abstract int Defense { get; }

    public abstract float MoveSpeedMult { get; }
    public abstract float JumpForceMult { get; }
    public abstract float GravityMult { get; }

    public abstract RuntimeAnimatorController AnimatorController { get; }
}
