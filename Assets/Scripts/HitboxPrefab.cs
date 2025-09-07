using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Skill から呼び出される、生成される Hitbox のプレハブにアタッチする
/// </summary>
public class HitboxPrefab : MonoBehaviour
{
    [Tooltip("正:ダメージ, 0:演出のみ, 負:回復")]
    [SerializeField] private int baseDamage;
    [Tooltip("生成されてから消えるまでの時間 (秒), アニメーター付きなら0にしておく")]
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

    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.isTrigger = true;

        // A. 生成されてから一定時間で消える
        if (lifetime > 0) Destroy(gameObject, lifetime);
    }

    public void SetStatus(PlayerCore owner, JudgeResult judgeResult)
    {
        this.owner = owner;
        this.judgeResult = judgeResult;
    }

    // B. ヒット判定を持ち、敵にダメージを与える(敵のPlayerCoreのHurtを呼び出す)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 敵のPlayerCoreを取得
        if (collision.TryGetComponent(out PlayerCore enemy))
        {

            Debug.Log($"Hitbox OnTriggerEnter2D with Player {enemy.PlayerId}");
            // 2. 自身、既にヒットした敵かどうかを判定
            if (enemy == owner || alreadyHitEnemies.Contains(enemy)) return;

            if (enemy.isInvincible.Value) return;

            if (baseDamage == 0) return;

            Debug.Log($"Hitbox hit Player {enemy.PlayerId} by Player {owner.PlayerId} with JudgeResult {judgeResult}");

            // 3. Hurtを呼び出す
            // 4. 自身に与えられたJudgeResultに応じてダメージを変化させる
            if (judgeResult == JudgeResult.Critical)
            {
                enemy.Hurt(owner, (int)Math.Ceiling(baseDamage * 2.0f), CalcKnockback(kbDegree, kbForce), true);
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
                enemy.Hurt(owner, (int)Math.Ceiling(baseDamage * 0.2f), CalcKnockback(kbDegree, kbForce), false);
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
        Vector2 knockbackDirection = Quaternion.Euler(0, 0, degree) * Vector2.right;
        return knockbackDirection * magnitude;
    }

    // # アニメーションイベント用
    public void DisableCollider()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
    }

    public void DestroyHitbox()
    {
        Destroy(gameObject);
    }
}
