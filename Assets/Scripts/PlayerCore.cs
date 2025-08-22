using System.ComponentModel;
using UnityEngine;
using UniRx;

public class PlayerCore : MonoBehaviour
{
    [SerializeField, ReadOnly(true)] private int playerId;
    public int PlayerId => playerId;

    public PlayerCore SetPlayerId(int id)
    {
        playerId = id;
        return this;
    }

    // * フラグ管理

    ReactiveProperty<bool> isAlive = new ReactiveProperty<bool>(true);
    ReactiveProperty<bool> isCombo = new ReactiveProperty<bool>(false);

    // TODO

    // * 移動系ステータス
    [SerializeField] private float moveSpeedMult = 1.0f;
    [SerializeField] private float jumpForceMult = 1.0f;
    [SerializeField] private float fallSpeedTerm = 1.0f;
    [SerializeField] private float gravityMult = 1.0f;

    // * 移動系
    public Subject<float> onMove = new Subject<float>();
    public void Move(float inputX)
    {
        onMove.OnNext(inputX * moveSpeedMult);
    }

    public Subject<float> onJump = new Subject<float>();
    public void Jump()
    {
        onJump.OnNext(jumpForceMult);
    }

    public Subject<float> onFall = new Subject<float>();
    public void Fall(float inputY)
    {
        onFall.OnNext(inputY);
    }

    // * スキル発動系
    public Subject<Unit> onSkillA = new Subject<Unit>();
    public void SkillA()
    {
        Debug.Log($"Skill A activated by Player ID: {playerId}");
        onSkillA.OnNext(Unit.Default);
    }

    public Subject<Unit> onSkillB = new Subject<Unit>();
    public void SkillB()
    {
        onSkillB.OnNext(Unit.Default);
    }
}
