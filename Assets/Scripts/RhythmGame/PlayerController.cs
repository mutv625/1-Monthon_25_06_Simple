using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1プレイヤー分のリズムゲーム要素（レーン、判定箇所、設定など）を管理するクラス。
/// 格闘ゲームのキャラクターの子オブジェクトにすることを想定。
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("レーン設定")]
    public float noteSpeed = 10f; // プレイヤーのノーツ速度

    // このレーン上で現在アクティブなノーツのGameObjectを管理するリスト
    public List<GameObject> activeNotes = new List<GameObject>();

    /// <summary>
    /// このレーン全体の位置と回転を、外部から設定する
    /// </summary>
    /// <param name="position">ワールド座標</param>
    /// <param name="angleZ">Z軸の回転角度</param>
    public void SetPose(Vector2 position, float angleZ)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 0, angleZ);
    }
}