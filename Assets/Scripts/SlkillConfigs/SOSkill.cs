using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// スキルのScriptableObjectを作る場合は必ずこのクラスを継承すること。
/// </summary>
public abstract class SOSkill : ScriptableObject
{
    public abstract string SkillName { get; }

    /// <summary>
    /// スキルの発動時間
    /// </summary>
    public abstract TimeSpan ActivationTime { get; }

    /// <summary>
    /// スキル固有の変数を初期化するためのメソッド。<br/>
    /// ScriptableObject は変数がゲーム終了後も保持されるため<br/>
    /// スキルのセット時に初期化処理を行う。<br/>
    /// </summary>
    public virtual void InitializeSkill() {}

    /// <summary>
    /// スキルを発動するときに呼び出されるメソッド。<br/>
    /// 当たり判定を飛ばすなどの処理はここで行う。
    /// </summary>
    /// <param name="skillPlayer">
    /// スキルを実行するプレイヤー
    /// </param>
    /// <param name="attackingState">
    /// なんのスキルとして発動されたか
    /// </param>
    public virtual async void Perform(PlayerCore skillPlayer)
    {
        await Task.Delay(ActivationTime);
    }

    public virtual void Execute(PlayerCore skillPlayer)
    {
        Perform(skillPlayer);

        // TODO: 攻撃状態の更新 (AttackingState.None に戻す)
        // ? 攻撃の終了はどうPlayerCoreに通知する？
    }
}   