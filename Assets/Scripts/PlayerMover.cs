using UnityEngine;
using UniRx;
using System.ComponentModel;

public class PlayerMover : MonoBehaviour
{
    // * 移動用定数
    // ゲーム内共通の値、キャラクターごとのものではない
    const float GLB_MOVE_SPEED = 10f;
    const float GLB_JUMP_FORCE = 8f;
    const float GLB_GRAVITY_SCALE = 13f;

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
        playerCore.onHurtAndKB
            .Subscribe(kbVec =>
            {
                AddImpulseVec(kbVec);
            });

        // * コンボ中パラメータ影響
        playerCore.comboState
            .Where(state => state == ComboStates.Combo)
            .Subscribe(_ =>
            {
                // コンボ中は移動系 1/3 倍
                glbMoveSpeed = GLB_MOVE_SPEED / 3f;
                glbJumpForce = GLB_JUMP_FORCE / 3f;
                glbGravityScale = GLB_GRAVITY_SCALE / 3f;
            });

        playerCore.comboState
            .Where(state => state == ComboStates.None)
            .Subscribe(_ =>
            {
                // 通常時に戻す
                glbMoveSpeed = GLB_MOVE_SPEED;
                glbJumpForce = GLB_JUMP_FORCE;
                glbGravityScale = GLB_GRAVITY_SCALE;
            });
        
        playerCore.comboState
            .Subscribe(state =>
            {
                if (state == ComboStates.Trapped) rb.simulated = false;
                else rb.simulated = true;
            });
    }

    [SerializeField] private float glbMoveSpeed = GLB_MOVE_SPEED;
    [SerializeField] private float glbJumpForce = GLB_JUMP_FORCE;
    [SerializeField] private float glbGravityScale = GLB_GRAVITY_SCALE;

    // 目標速度、終端速度みたいなイメージ
    [SerializeField] private float movementX;
    [SerializeField] private float movementY;

    // * 移動系
    public void MoveX(float inputX)
    {
        movementX = inputX * glbMoveSpeed;
    }

    public void MoveY(float inputY)
    {
        movementY = inputY * glbMoveSpeed;
    }

    // * 力を加える系
    public void AddImpulseVec(Vector2 impulse)
    {
        rb.linearVelocity = rb.linearVelocity / 2 + impulse;
    }

    public void AddImpulseY(float impulseY)
    {
        rb.linearVelocityY = rb.linearVelocityY / 2 + impulseY * glbJumpForce;
    }


    // * 移動の更新
    private void UpdateMovement()
    {
        rb.linearVelocityX = rb.linearVelocityX + (movementX - rb.linearVelocityX) * 0.2f * Time.deltaTime * 60;
        rb.linearVelocityY = rb.linearVelocityY - glbGravityScale * Time.deltaTime;
    }
}
