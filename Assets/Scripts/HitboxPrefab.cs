using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Skill から呼び出される、生成される Hitbox のプレハブにアタッチする
/// </summary>
public class HitboxPrefab : MonoBehaviour
{
    [SerializeField] private int baseDamage;
    [SerializeField] private float lifetime;

    [Header("閲覧専用")]
    [SerializeField] public PlayerCore owner;
    [SerializeField] private List<PlayerCore> alreadyHitEnemies = new List<PlayerCore>();

    // TODO: Hitboxの役割
    // 1. 生成されてから一定時間で消える
    // 2. ヒット判定を持ち、敵にダメージを与える(敵のPlayerCoreのHurtを呼び出す)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();  
        collider.isTrigger = true;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: ヒット判定処理
        // 1. 敵のPlayerCoreを取得
        if (collision.TryGetComponent(out PlayerCore enemy))
        {
            // 2. 自身、既にヒットした敵かどうかを判定
            if (enemy == owner || alreadyHitEnemies.Contains(enemy)) return;

            // 3. Hurtを呼び出す // ? KBも設定できるようにするか
            enemy.Hurt(baseDamage);
            alreadyHitEnemies.Add(enemy);
        }
    }
}
