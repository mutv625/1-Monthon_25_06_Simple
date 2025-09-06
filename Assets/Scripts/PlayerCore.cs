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

    [SerializeField] private JudgeProvider judgeProvider;

    // TODO: FighterSO から読み込んでステータスの初期化

    public PlayerCore SetPlayerId(int id)
    {
        playerId = id;

        // TODO: IDが奇数なら向きを左にする
        if (playerId % 2 == 1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingDirection.Value = Vector2.left;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingDirection.Value = Vector2.right;
        }

        return this;
    }

    public void SetFighterSO(SOFighterPayload so)
    {
        maxHealth.Value = so.MaxHealth;
        currentHealth.Value = so.MaxHealth;

        moveSpeedMult = so.MoveSpeed;
        jumpForceMult = so.JumpForce;
        gravityMult = so.Gravity;
    }
    
    private IDisposable comboDepletionTimer;

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        fightingEP = fightingEntryPoint;

        judgeProvider = GetComponent<JudgeProvider>();

        // * コンボ状態が変化したときの処理
        comboState.Pairwise()
            .Where(pair => pair.Previous == ComboStates.None && pair.Current == ComboStates.Combo)
            .Subscribe(_ =>
            {
                OnStartCombo();
                Debug.Log($"Player {playerId} started Combo!");
            }).AddTo(this);

        comboState.Pairwise()
            .Where(pair => pair.Previous == ComboStates.None && pair.Current == ComboStates.Trapped)
            .Subscribe(_ =>
            {
                OnStartTrapped();
                Debug.Log($"Player {playerId} got Trapped!");
            }).AddTo(this);

        onSkill
            .Where(_ => comboState.Value == ComboStates.Combo)
            .Subscribe(status => ChangeComboGaugeByJudge(status.Item2))
            .AddTo(this);

        fightingEP.updateInFighting
            .Where(_ => comboState.Value == ComboStates.Combo)
            .Subscribe(_ => ChangeComboGaugeByTime())
            .AddTo(this);

        comboState.DistinctUntilChanged()
            .Where(state => state == ComboStates.None)
            .Subscribe(_ => ResetComboStatus())
            .AddTo(this);

        comboGaugeValue
            .Select(v => v <= 0f)          // bool に変換
            .DistinctUntilChanged()        // 状態変化のみ
            .Subscribe(isZero =>
            {
                if (isZero && comboState.Value == ComboStates.Combo)
                {
                    // 既存のタイマーをキャンセル（再度ダメージが来た場合にリスタートする）
                    comboDepletionTimer?.Dispose();
                    comboDepletionTimer = Observable.Timer(TimeSpan.FromSeconds(0.1))
                        .Subscribe(__ =>
                        {
                            // タイマー完了時に改めて条件を確認してから発火
                            if (comboGaugeValue.Value <= 0f && comboState.Value == ComboStates.Combo)
                            {
                                Debug.Log($"Player {playerId} combo gauge depleted after 0.1s.");
                                fightingEP.FinishComboForEveryone();
                            }
                            // 終了したので参照をクリア
                            comboDepletionTimer?.Dispose();
                            comboDepletionTimer = null;
                        });
                }
                else
                {
                    // false（ゲージ回復）になったらタイマーをキャンセル
                    comboDepletionTimer?.Dispose();
                    comboDepletionTimer = null;
                }
            })
            .AddTo(this);

        // Endingの状態で1秒経過したらNoneにする
        comboState.Pairwise()
            .Where(pair => pair.Previous == ComboStates.Combo && pair.Current == ComboStates.Ending)
            .Subscribe(_ =>
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(__ =>
                {
                    if (comboState.Value == ComboStates.Ending)
                    {
                        comboState.Value = ComboStates.None;
                        Debug.Log($"Player {playerId} combo ended after Ending state.");
                    }
                }).AddTo(this);
            }).AddTo(this);
    }

    // * 基本ステータス
    [Header("基本ステータス")]
    [SerializeField] public IntReactiveProperty maxHealth = new IntReactiveProperty(1000);
    [SerializeField] public IntReactiveProperty currentHealth = new IntReactiveProperty(1000);

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

        // 判定を取得
        JudgeResult jr = judgeProvider.Judge(playerId);
        Debug.Log($"Player {playerId} SkillA JudgeResult: {jr}");

        if (jr == JudgeResult.Critical || jr == JudgeResult.Perfect)
        {
            attackingState.Value = AttackingStates.SkillAx;
            onSkill.OnNext((attackingState.Value, jr));
            Debug.Log($"Player {playerId} has {attackingState.Value} state.");
        }
        else
        {
            attackingState.Value = AttackingStates.SkillA;
            onSkill.OnNext((attackingState.Value, jr));
            Debug.Log($"Player {playerId} has {attackingState.Value} state.");
        }
    }

    public void SkillB()
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

        // 判定を取得
        JudgeResult jr = judgeProvider.Judge(playerId);
        Debug.Log($"Player {playerId} SkillB JudgeResult: {jr}");

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

    // # 被ダメージ時の処理 + コンボ突入

    // * コンボダメージ計算用
    // TODO: 選択難易度によって変更
    float NUMER = 1.5f;
    float DENOM = 0.9f;
    float EBASE = 1.17f;
    int ADDITIONAL = 20;

    public Subject<Vector2> onHurtAndKB = new Subject<Vector2>();
    public void Hurt(PlayerCore attacker, int damage, Vector2 kbVec, bool doStartCombo)
    {
        // -2. ダメージ受け始めの処理 (ダメージモーション開始など)
        isHurting.Value = true;

        // # ダメージ計算
        int finalDamage;

        if (comboState.Value == ComboStates.Trapped)
        {
            finalDamage = (int)(damage * NUMER / (DENOM + MathF.Pow(EBASE, comboTrappedCount.Value))) + ADDITIONAL;
        }
        else
        {
            finalDamage = damage;
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

        if (attacker.comboState.Value == ComboStates.Combo)
        {
            comboTrappedCount.Value += 1;
            attacker.comboDamage.Value += finalDamage;
            attacker.comboCount.Value += 1;
        }

        // 0. 死亡判定
        if (currentHealth.Value - finalDamage <= 0)
        {
            Debug.Log($"Player {playerId} will die.");
            currentHealth.Value = 0;
            return;
        }

        // 2. ダメージの適用
        Debug.Log($"P{attacker.playerId} >> P{playerId} ( {finalDamage}dmg ).");
        currentHealth.Value -= finalDamage;

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
    [SerializeField] public FloatReactiveProperty comboGaugeValue = new FloatReactiveProperty(0f);
    public float ComboGaugeValue
    {
        get => comboGaugeValue.Value;
        private set => comboGaugeValue.Value = Mathf.Clamp(value, 0f, 100f);
    }

    [SerializeField] float comboElapsedTime = 0f;

    [SerializeField] public IntReactiveProperty comboCount = new IntReactiveProperty(0);
    [SerializeField] public IntReactiveProperty comboDamage = new IntReactiveProperty(0);

    [SerializeField] IntReactiveProperty comboTrappedCount = new IntReactiveProperty(0);


    // ComboStatus 値が Combo になった瞬間に呼び出される
    private void OnStartCombo()
    {
        ComboGaugeValue = 100f;
        comboElapsedTime = 0f;
    }

    private void OnStartTrapped()
    {
        comboTrappedCount.Value = 1;
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
        float delta = -Time.deltaTime * (5f + 5f * (float)Math.Pow(comboElapsedTime,0.75) / 2f);

        ComboGaugeValue += delta;
    }

    // # コンボ終了処理
    public void ResetComboStatus()
    {
        comboElapsedTime = 0f;
        comboTrappedCount.Value = 0;

        comboDamage.Value = 0;
        comboCount.Value = 0;
    }
}
