using System.ComponentModel;
using UnityEngine;
using UniRx;
using System;

public enum AttackingState {
    None,
    SkillA,
    SkillB
}

public class PlayerCore : MonoBehaviour
{
    [Header("プレイヤー識別用")]
    [SerializeField] private int playerId;
    public int PlayerId => playerId;

    public PlayerCore SetPlayerId(int id)
    {
        playerId = id;
        return this;
    }


    // * フラグ管理
    [Header("フラグ管理")]
    [SerializeField] BoolReactiveProperty isAlive = new BoolReactiveProperty(true);
    [SerializeField] BoolReactiveProperty isInvincible = new BoolReactiveProperty(false);

    [SerializeField] BoolReactiveProperty isCombo = new BoolReactiveProperty(false);
    [SerializeField] BoolReactiveProperty isHurting = new BoolReactiveProperty(false);
    [SerializeField] ReactiveProperty<AttackingState> attackingState = new ReactiveProperty<AttackingState>(AttackingState.None);
    [SerializeField] IntReactiveProperty jumpCount = new IntReactiveProperty(0);
    [SerializeField] BoolReactiveProperty isAppearing = new BoolReactiveProperty(false);
    [SerializeField] BoolReactiveProperty isDashing = new BoolReactiveProperty(false);

    [SerializeField] ReactiveProperty<Vector2> facingDirection = new ReactiveProperty<Vector2>(Vector2.right);


    // * 移動系ステータス
    [Header("移動系ステータス")]
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

    // * アニメーション用
    
}
