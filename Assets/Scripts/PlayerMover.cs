using UnityEngine;
using UniRx;
using System.ComponentModel;

public class PlayerMover : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerCore playerCore;

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        rb = GetComponent<Rigidbody2D>();
        playerCore = GetComponent<PlayerCore>();

        fightingEntryPoint.updateInFighting
            .Subscribe(_ => UpdateMovement());

        playerCore.onMove.Subscribe(inputX => MoveX(inputX));
        playerCore.onJump.Subscribe(jumpMult => AddImpulseY(jumpMult));
        playerCore.onFall.Subscribe(inputY => MoveY(inputY));
    }

    // * 移動用定数
    // ゲーム内共通の値、キャラクターごとのものではない
    [SerializeField] private readonly float MOVE_SPEED = 10f;
    [SerializeField] private readonly float JUMP_FORCE = 8f;
    [SerializeField] private readonly float GRAVITY_SCALE = 13f;

    // 目標速度、終端速度みたいなイメージ
    [SerializeField, ReadOnly(true)] private float movementX;
    [SerializeField, ReadOnly(true)] private float movementY;

    public void MoveX(float inputX)
    {
        movementX = inputX * MOVE_SPEED;
    }

    public void MoveY(float inputY)
    {
        movementY = inputY * MOVE_SPEED;
    }

    public void AddImpulseY(float impulseY)
    {
        rb.linearVelocityY = rb.linearVelocityY / 2 + impulseY * JUMP_FORCE;
    }

    private void UpdateMovement()
    {
        rb.linearVelocityX = rb.linearVelocityX + (movementX - rb.linearVelocityX) * 0.4f * Time.deltaTime * 60;
        rb.linearVelocityY = rb.linearVelocityY - GRAVITY_SCALE * Time.deltaTime;
    }
}
