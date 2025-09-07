using UnityEngine;

/// <summary>
/// 個々のノーツの振る舞いを制御するクラス。
/// ノーツのプレハブ（NormalNote_Prefab, LargeNote_Prefab）のルートにアタッチします。
/// </summary>

public class NoteController : MonoBehaviour
{
    // RhythmGameManagerから設定される情報
    public float speed;
    public float judgeTime;
    public NoteType type;
    public bool isDebug = false;

    private Transform visualsTransform; // 見た目を担当する子オブジェクト

    void Awake()
    {
        visualsTransform = transform.Find("Visuals");
    }

    void Update()
    {
        // 自身のローカル座標系で「下」方向に移動し続ける
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.Self);

        // 親が回転しても、見た目だけは回転しないようにする
        if (visualsTransform != null)
        {
            visualsTransform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 判定成功時にRhythmGameManagerから呼び出される
    /// </summary>

    public void OnHit()
    {
        // isDebugがtrueの時だけログを出力
        if (isDebug)
        {
            Debug.Log($"--- DESTROYING myself via OnHit. My ID is: {gameObject.GetInstanceID()} ---", gameObject);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// 見逃し時にRhythmGameManagerから呼び出される
    /// </summary>

    public void OnMiss()
    {
        // isDebugがtrueの時だけログを出力
        if (isDebug)
        {
            Debug.Log($"--- DESTROYING myself via OnMiss. My ID is: {gameObject.GetInstanceID()} ---", gameObject);
        }
        Destroy(gameObject);
    }
}