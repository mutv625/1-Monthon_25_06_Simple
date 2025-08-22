using UnityEngine;

/// <summary>
/// スキルのScriptableObjectを作る場合は必ずこのクラスを継承すること。
/// </summary>
public abstract class SOSkill : ScriptableObject
{
    public abstract string SkillName { get; }

    /// <summary>
    /// スキルを初期化するためのメソッド。<br/>
    /// ScriptableObject は変数がゲーム終了後も保持されるため<br/>
    /// スキルのセット時に初期化処理を行う。<br/>
    /// </summary>
    /// ... しかしスキルはこれのコピーを使うので、別に保存されないことに気づく
    // public abstract void InitializeSkill();

    /// <summary>
    /// スキルを発動するときに呼び出されるメソッド。<br/>
    /// 当たり判定を飛ばすなどの処理はここで行う。
    /// </summary>
    /// <param name="executor">
    /// スキルを実行するプレイヤー
    /// </param>
    public abstract void ExecuteSkill(PlayerCore executor);
}   