using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Skill から呼び出される、生成される Hitbox のプレハブにアタッチする
/// </summary>
public class HitboxPrefab : MonoBehaviour
{
    [SerializeField] private int baseDamage;
    [SerializeField] private float lifetime;

    /// <summary>
    /// 向いている方向からの角度 (0 = 横, 90 = 真上, 180 = 後ろ)
    /// </summary>
    [Tooltip("向いている方向からの角度 (0 = 横, 90 = 真上, 180 = 後ろ)")]
    [SerializeField] private float kbDegree;
    [SerializeField] private float kbForce;


    [Header("閲覧専用")]
    [SerializeField] public PlayerCore owner;
    [SerializeField] private List<PlayerCore> alreadyHitEnemies = new List<PlayerCore>();

    [SerializeField] private JudgeResult judgeResult;

    // TODO: Hitboxの役割
    // 1. 生成されてから一定時間で消える
    // 2. ヒット判定を持ち、敵にダメージを与える(敵のPlayerCoreのHurtを呼び出す)

    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    public void SetStatus(PlayerCore owner, JudgeResult judgeResult)
    {
        this.owner = owner;
        this.judgeResult = judgeResult;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 敵のPlayerCoreを取得
        if (collision.TryGetComponent(out PlayerCore enemy))
        {
            // 2. 自身、既にヒットした敵かどうかを判定
            if (enemy == owner || alreadyHitEnemies.Contains(enemy)) return;

            if (enemy.isInvincible.Value) return;

            // 3. Hurtを呼び出す
            // 4. 自身に与えられたJudgeResultに応じてダメージを変化させる
            // ! TODO: Crit を相手に当てると コンボが終了した途端コンボが始まり、終わらないらしいんですが
            if (judgeResult == JudgeResult.Critical)
            {
                enemy.Hurt(owner, baseDamage * 2, CalcKnockback(kbDegree, kbForce), true);
            }
            else if (judgeResult == JudgeResult.Perfect)
            {
                enemy.Hurt(owner, (int)Math.Ceiling(baseDamage * 1.5f), CalcKnockback(kbDegree, kbForce), true);
            }
            else if (judgeResult == JudgeResult.Good)
            {
                enemy.Hurt(owner, (int)Math.Ceiling(baseDamage * 1.2f), CalcKnockback(kbDegree, kbForce), false);
            }
            else if (judgeResult == JudgeResult.None && owner.comboState.Value == ComboStates.Combo)
            {
                // コンボ中のNone判定
                enemy.Hurt(owner, (int)Math.Ceiling(baseDamage * 0.2f), Vector2.zero, false);
            }
            else
            {
                enemy.Hurt(owner, baseDamage, CalcKnockback(kbDegree, kbForce), false);
            }


            alreadyHitEnemies.Add(enemy);
        }
    }

    private Vector2 CalcKnockback(float degree, float magnitude)
    {
        // TODO: Knockbackの計算
        Vector2 knockbackDirection = Quaternion.Euler(0, 0, degree) * Vector2.right;
        return knockbackDirection * magnitude;
    }
}
