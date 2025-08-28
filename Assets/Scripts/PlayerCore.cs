using System.ComponentModel;
using UnityEngine;
using UniRx;
using System;

public enum AttackingStates
{
    None,
    SkillA,
    SkillB
}

public enum ComboStates
{
    Trapped = -1,
    None = 0,
    Combo = 1
}

public class PlayerCore : MonoBehaviour
{
    [Header("プレイヤー識別用")]
    [SerializeField] private int playerId;
    public int PlayerId => playerId;

    [SerializeField] private FightingEntryPoint fightingEP;

    public PlayerCore SetPlayerId(int id)
    {
        playerId = id;
        return this;
    }

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        this.fightingEP = fightingEntryPoint;


        // フラグを時間経過で戻すイベントたち 
    }

    // * 基本ステータス
    [Header("基本ステータス")]
    [SerializeField] IntReactiveProperty maxHealth = new IntReactiveProperty(100);
    [SerializeField] IntReactiveProperty currentHealth = new IntReactiveProperty(100);

    // * フラグ管理
    [Header("フラグ管理")]
    [SerializeField] BoolReactiveProperty isInvincible = new BoolReactiveProperty(false);

    [SerializeField] ReactiveProperty<ComboStates> isCombo = new ReactiveProperty<ComboStates>(ComboStates.None);
    [SerializeField] BoolReactiveProperty isHurting = new BoolReactiveProperty(false);
    [SerializeField] public ReactiveProperty<AttackingStates> attackingState = new ReactiveProperty<AttackingStates>(AttackingStates.None);
    [SerializeField] public IntReactiveProperty jumpCount = new IntReactiveProperty(0);
    [SerializeField] BoolReactiveProperty isAppearing = new BoolReactiveProperty(false);
    [SerializeField] public BoolReactiveProperty isDashing = new BoolReactiveProperty(false);


    // TODO: 常にこれを基準にプレイヤーの反転や向き、アニメーションの向きを決定する
    // ! スキル中は変化しない
    [SerializeField] ReactiveProperty<Vector2> facingDirection = new ReactiveProperty<Vector2>(Vector2.right);
    // * 移動系ステータス
    [Header("移動系ステータス")]
    [SerializeField] private float moveSpeedMult = 1.0f;
    [SerializeField] private float jumpForceMult = 1.0f;
    [SerializeField] private float fallSpeedTerm = 1.0f;
    [SerializeField] private float gravityMult = 1.0f;

    // ! 以下の関数ではプレイヤー自身のフラグを変更する

    // * 移動系
    public Subject<float> onMove = new Subject<float>();
    public void Move(float inputX)
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            isDashing.Value = false;
            onMove.OnNext(0);
            return;
        }

        if (inputX != 0)
        {
            if (Mathf.Sign(inputX) != Mathf.Sign(facingDirection.Value.x))
            {
                facingDirection.Value = new Vector2(Mathf.Sign(inputX), 0);
            }
            if (!isDashing.Value)
            {
                isDashing.Value = true;
            }
        }
        else
        {
            isDashing.Value = false;
        }

        onMove.OnNext(inputX * moveSpeedMult);
    }

    public Subject<float> onJump = new Subject<float>();
    public void Jump()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }


        if (jumpCount.Value < 2 && attackingState.Value == AttackingStates.None)
        {
            onJump.OnNext(jumpForceMult);
            jumpCount.Value += 1;
        }
    }

    public void ResetJumpCount()
    {
        if (jumpCount.Value != 0) jumpCount.Value = 0;
    }

    public Subject<float> onFall = new Subject<float>();
    public void Fall(float inputY)
    {
        onFall.OnNext(inputY);
    }


    // * スキル発動系
    public Subject<AttackingStates> onSkill = new Subject<AttackingStates>();
    // スキルキーが押されたときに呼び出される
    public void SkillA()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }

        attackingState.Value = AttackingStates.SkillA;
        onSkill.OnNext(attackingState.Value);
    }

    public void SkillB()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }

        attackingState.Value = AttackingStates.SkillB;
        onSkill.OnNext(attackingState.Value);
    }

    public Subject<Unit> onHurt = new Subject<Unit>();
    public void Hurt(int damage)
    {
        onHurt.OnNext(Unit.Default);
    }

    // ノックバックも加えたダメージ処理
    public void HurtWithKB(Vector2 knockbackDirection)
    {
        onHurt.OnNext(Unit.Default);
        // TODO: ノックバック処理 PlayerMover.ImpulseVector() 行きか?
    }

    public void ResetLockingStatusAtCore()
    {
        attackingState.Value = AttackingStates.None;
        isHurting.Value = false;
    }
}

