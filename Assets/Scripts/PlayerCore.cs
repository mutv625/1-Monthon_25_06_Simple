using System.ComponentModel;
using UnityEngine;
using UniRx;
using System;

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
        fightingEP = fightingEntryPoint;

        // * コンボ状態が変化したときの処理
        comboState.Pairwise()
            .Where(pair => pair.Previous == ComboStates.None && pair.Current == ComboStates.Combo)
            .Subscribe(_ =>
            {
                OnStartCombo();
                Debug.Log($"Player {playerId} started Combo!");
            });

        onSkill
            .Where(_ => comboState.Value == ComboStates.Combo)
            .Subscribe(status => ChangeComboGaugeByJudge(status.Item2));

        fightingEP.updateInFighting
            .Where(_ => comboState.Value == ComboStates.Combo)
            .Subscribe(_ => ChangeComboGaugeByTime());

        comboState.DistinctUntilChanged()
            .Where(state => state == ComboStates.None)
            .Subscribe(_ => FinishCombo());

        comboGaugeValue
            .Where(value => value <= 0f && comboState.Value == ComboStates.Combo)
            .Subscribe(_ =>
            {
                fightingEP.FinishComboForEveryone();
                Debug.Log($"Player {playerId} combo gauge depleted, all combos finished.");
            });
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


    // TODO: 常にこれを基準にプレイヤーの反転(ScaleX)や向き、アニメーションの向きを決定する
    // ! スキル中は変化しない
    [SerializeField] ReactiveProperty<Vector2> facingDirection = new ReactiveProperty<Vector2>(Vector2.right);
    // * 移動系ステータス
    [Header("移動系ステータス")]
    [SerializeField] private float moveSpeedMult = 1.0f;
    [SerializeField] private float jumpForceMult = 1.0f;
    [SerializeField] private float fallSpeedTerm = 1.0f;
    [SerializeField] private float gravityMult = 1.0f;

    // ! 以下の関数ではプレイヤー自身のフラグを変更する

    // # 移動系
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

    public Subject<(float,int)> onJump = new Subject<(float,int)>();
    public void Jump()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            return;
        }


        if (jumpCount.Value < 2 && attackingState.Value == AttackingStates.None)
        {
            jumpCount.Value += 1;
            onJump.OnNext((jumpForceMult, jumpCount.Value));
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
    [SerializeField] private JudgeResult recentJudgeResult = JudgeResult.None;

    // # スキル発動系
    public Subject<(AttackingStates, JudgeResult)> onSkill = new Subject<(AttackingStates, JudgeResult)>();
    // スキルキーが押されたときに呼び出される
    public void SkillA()
    {
        // ! スキル、ダメージ中、被コンボ中はロック
        // ! ただし、コンボ中はスキルを発動できる
        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            if (comboState.Value == ComboStates.Combo)
            {
                // ! コンボ中はスキルを発動できる
            }
            else
            {
                return;
            }
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

        if (attackingState.Value != AttackingStates.None || isHurting.Value)
        {
            if (comboState.Value == ComboStates.Combo)
            {
                // ! コンボ中はスキルを発動できる
            }
            else
            {
                return;
            }
        }
    }

    // # 被ダメージ時の処理 + コンボ突入
    public Subject<Vector2> onHurtAndKB = new Subject<Vector2>();
    public void Hurt(PlayerCore attacker, int damage, Vector2 kbVec, bool doStartCombo)
    {

        isHurting.Value = true;

        // 0. 死亡判定
        if (currentHealth.Value - damage <= 0)
        {
            Debug.Log($"Player {playerId} will die.");
            currentHealth.Value = 0;
            return;
        }

        // * 以下 not死亡時の処理
        // 1. コンボ系の処理
        // 両者の ComboState を参照し、物理演算をここで有効無効を切り替える
        if (doStartCombo)
        {
            if (attacker.comboState.Value == ComboStates.None && comboState.Value == ComboStates.None)
            {
                attacker.comboState.Value = ComboStates.Combo;
                comboState.Value = ComboStates.Trapped;
            }
            
        }

        // 2. ダメージの適用
        Debug.Log($"P{attacker.playerId} >> P{playerId} ( {damage}dmg ).");
        currentHealth.Value -= damage;

        // 3. Knockbackの適用
        // PlayerMover側で、コンボ中は物理演算を停止し、ノックバックを1/3にする
        // その結果、KBは蓄積され、物理演算再開で超ふっとぶ
        onHurtAndKB.OnNext(new Vector2(kbVec.x * transform.localScale.x, kbVec.y));
    }

    // ! 技アニメ、被ダメージアニメ終了時に呼び出すこと
    // ステータスのロックを解除する
    public void ResetLockingStatusAtCore()
    {
        attackingState.Value = AttackingStates.None;
        isHurting.Value = false;
        // TODO: Ending 状態の扱いを真面目に考える
        if (comboState.Value == ComboStates.Ending)
        {
            comboState.Value = ComboStates.None;
        }
    }

    // HP<=0 で呼び出す ~死亡~
    public void DIE()
    {
        Debug.Log($"Player {playerId} died.");
        // TODO: 死亡処理
    }

    // # 与コンボ開始時の処理
    [SerializeField] FloatReactiveProperty comboGaugeValue = new FloatReactiveProperty(0f);
    private float ComboGaugeValue
    {
        get => comboGaugeValue.Value;
        set => comboGaugeValue.Value = Mathf.Clamp(value, 0f, 100f);
    }

    [SerializeField] float comboElapsedTime = 0f;
    [SerializeField] IntReactiveProperty comboCount = new IntReactiveProperty(0);


    // ComboStatus 値が Combo になった瞬間に呼び出される
    private void OnStartCombo()
    {
        ComboGaugeValue = 100f;
        comboElapsedTime = 0f;
        comboCount.Value = 0;
    }

    // # コンボ中のゲージ変化
    // (呼び出されているのは今のところ onSkill)
    private void ChangeComboGaugeByJudge(JudgeResult jr)
    {
        float delta = jr switch
        {
            JudgeResult.Critical => -100f,
            JudgeResult.Perfect => 10f,
            JudgeResult.Good => 5f,
            JudgeResult.None => -5f,
            JudgeResult.Miss => -10f,
            _ => 0f
        };

        ComboGaugeValue += delta;
    }

    private void ChangeComboGaugeByTime()
    {
        comboElapsedTime += Time.deltaTime;
        float delta = -Time.deltaTime * (5f + 5f * (float)Math.Sqrt(comboElapsedTime) / 2f);

        ComboGaugeValue += delta;
    }

    // # コンボ終了処理
    public void FinishCombo()
    {
        comboElapsedTime = 0f;
        comboCount.Value = 0;
    }
}
