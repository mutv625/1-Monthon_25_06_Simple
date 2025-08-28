using UnityEngine;

/// <summary>
/// TODO: Skill から呼び出される、生成される Hitbox のプレハブにアタッチする
/// </summary>
public class HitboxPrefab : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float lifetime;

    // TODO: Hitboxの役割
    // 1. 生成されてから一定時間で消える
    // 2. ヒット判定を持ち、敵にダメージを与える(敵のPlayerCoreのHurtを呼び出す)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();            
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: ヒット判定処理
        // 1. 敵のPlayerCoreを取得
        if (collision.TryGetComponent(out PlayerCore enemy))
        {
            // 2. Hurtを呼び出す // ? KBも設定できるようにするか
            enemy.Hurt(damage);
        }
    }
}
