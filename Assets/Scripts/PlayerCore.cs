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

    // * 基本ステータス
    [Header("基本ステータス")]
    [SerializeField] IntReactiveProperty maxHealth = new IntReactiveProperty(100);
    [SerializeField] IntReactiveProperty currentHealth = new IntReactiveProperty(100);
    [SerializeField] IntReactiveProperty defense = new IntReactiveProperty(0);

    // * フラグ管理
    [Header("フラグ管理")]
    [SerializeField] BoolReactiveProperty isInvincible = new BoolReactiveProperty(false);

    [SerializeField] BoolReactiveProperty isCombo = new BoolReactiveProperty(false);
    [SerializeField] BoolReactiveProperty isHurting = new BoolReactiveProperty(false);
    [SerializeField] ReactiveProperty<AttackingState> attackingState = new ReactiveProperty<AttackingState>(AttackingState.None);
    [SerializeField] IntReactiveProperty jumpCount = new IntReactiveProperty(0);
    [SerializeField] BoolReactiveProperty isAppearing = new BoolReactiveProperty(false);
    [SerializeField] BoolReactiveProperty isDashing = new BoolReactiveProperty(false);
    public IReadOnlyReactiveProperty<bool> IsDashing => isDashing;

    [SerializeField] ReactiveProperty<Vector2> facingDirection = new ReactiveProperty<Vector2>(Vector2.right);


    [SerializeField] GroundChecker groundChecker;

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

    public void SkillA()
    {

    }

    public void SkillB()
    {

    }

    // * アニメーション用

}
