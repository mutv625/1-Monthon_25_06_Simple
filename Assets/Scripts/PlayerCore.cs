using System.ComponentModel;
using UnityEngine;
using UniRx;
using System;

public enum AttackingStates
{
    None,
    SkillA,
    SkillB,
    SkillAx,
    SkillBx
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

    // TODO: FighterSO から読み込んでステータスの初期化

    public PlayerCore SetPlayerId(int id)
    {
        playerId = id;

        // TODO: IDが奇数なら向きを左にする
        if (playerId % 2 == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        return this;
    }

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        this.fightingEP = fightingEntryPoint;


        // フラグを時間経過で戻すイベントたち 
    }

    // * 基本ステータス
    [Header("基本ステータス")]
    [SerializeField] IntReactiveProperty maxHealth = new IntReactiveProperty(1000);
    [SerializeField] IntReactiveProperty currentHealth = new IntReactiveProperty(1000);

    // * フラグ管理
    [Header("フラグ管理")]
    [SerializeField] public BoolReactiveProperty isInvincible = new BoolReactiveProperty(false);

    [SerializeField] public ReactiveProperty<ComboStates> comboState = new ReactiveProperty<ComboStates>(ComboStates.None);
    [SerializeField] public BoolReactiveProperty isHurting = new BoolReactiveProperty(false);
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


    [Header("判定テスト用")]
    [SerializeField] private JudgeResult recentJudgeResult;


    // * スキル発動系
    public Subject<(AttackingStates, JudgeResult)> onSkill = new Subject<(AttackingStates, JudgeResult)>();
    // スキルキーが押されたときに呼び出される
    public void SkillA()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }

        // TODO: 判定を取得
        JudgeResult jr = recentJudgeResult;

        if (jr == JudgeResult.Critical || jr == JudgeResult.Perfect)
        {
            attackingState.Value = AttackingStates.SkillAx;
            onSkill.OnNext((attackingState.Value, jr));
        }
        else
        {
            attackingState.Value = AttackingStates.SkillA;
            onSkill.OnNext((attackingState.Value, jr));
        }
    }

    public void SkillB()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }
        // TODO: 判定を取得
        JudgeResult jr = recentJudgeResult;

        if (jr == JudgeResult.Critical || jr == JudgeResult.Perfect)
        {
            attackingState.Value = AttackingStates.SkillBx;
            onSkill.OnNext((attackingState.Value, jr));
        }
        else
        {
            attackingState.Value = AttackingStates.SkillB;
            onSkill.OnNext((attackingState.Value, jr));
        }
    }

    // * 被ダメージ時の処理
    public Subject<Vector2> onHurtAndKB = new Subject<Vector2>();
    public void Hurt(PlayerCore attacker, int damage, Vector2 kbVec, bool doStartCombo)
    {
        
        isHurting.Value = true;

        // TODO 0. 死亡判定
        if (currentHealth.Value - damage <= 0)
        {
            Debug.Log($"Player {playerId} will die.");
            currentHealth.Value = 0;
            return;
        }

        // * 以下非死亡時の処理
        // TODO 1. コンボ系の処理
        // ComboState を参照し、物理演算をここで有効無効を切り替える
        if (doStartCombo)
        {
            comboState.Value = ComboStates.Trapped;
        }

        // TODO 2. ダメージの適用
        Debug.Log($"Player {playerId} hurts! Damage = {damage}");

        currentHealth.Value -= damage;

        // TODO 3. Knockbackの適用
        // PlayerMover側で、コンボ中は物理演算を停止し、ノックバックを1/3にする
        // その結果、KBは蓄積され、物理演算再開で超ふっとぶ
        onHurtAndKB.OnNext(new Vector2(kbVec.x * transform.localScale.x, kbVec.y));
    }

    // ! 技、被ダメージ終了時に呼び出され、ステータスのロックを解除する
    public void ResetLockingStatusAtCore()
    {
        attackingState.Value = AttackingStates.None;
        isHurting.Value = false;
    }

    // HP<=0 で呼び出す ~死亡~
    public void DIE()
    {
        Debug.Log($"Player {playerId} died.");
        // TODO: 死亡処理
    }
}

