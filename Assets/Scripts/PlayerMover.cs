using UnityEngine;
using UniRx;
using System.ComponentModel;
using System;

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
            .Subscribe(_ => UpdateMovement())
            .AddTo(this);

        playerCore.onMove.Subscribe(inputX => MoveX(inputX))
            .AddTo(this);
        playerCore.onJump.Subscribe(jumpStatus => AddImpulseY(jumpStatus.Item1))
            .AddTo(this);
        playerCore.onFall.Subscribe(inputY => MoveY(inputY))
            .AddTo(this);
        playerCore.onHurtAndKB
            .Subscribe(kbVec => AddImpulseVec(kbVec)).AddTo(this);

        // * コンボ中パラメータ影響
        playerCore.comboState
            .Where(state => state == ComboStates.Combo)
            .Subscribe(_ =>
            {
                // コンボ中は移動系 1/3 倍
                glbMoveSpeed = GLB_MOVE_SPEED / 3f;
                glbJumpForce = GLB_JUMP_FORCE / 3f;
                glbGravityScale = GLB_GRAVITY_SCALE / 3f;
            }).AddTo(this);

        playerCore.comboState
            .Where(state => state == ComboStates.None)
            .Subscribe(_ =>
            {
                // 通常時に戻す
                glbMoveSpeed = GLB_MOVE_SPEED;
                glbJumpForce = GLB_JUMP_FORCE;
                glbGravityScale = GLB_GRAVITY_SCALE;
            }).AddTo(this);
        
        playerCore.comboState
            .Subscribe(state =>
            {
                if (state == ComboStates.Trapped)
                {
                    // 拘束中は物理演算オフ
                    rb.bodyType = RigidbodyType2D.Static;
                }
                else
                {
                    // それ以外は物理演算オン
                    rb.bodyType = RigidbodyType2D.Dynamic;

                    // 蓄積したInpulseを加える
                    if (storedImpulseX != 0 || storedImpulseY != 0)
                    {
                        rb.linearVelocity = rb.linearVelocity / 2.5f + new Vector2(storedImpulseX, storedImpulseY);
                        storedImpulseX = 0;
                        storedImpulseY = 0;
                    }
                }
            }).AddTo(this);
    }

    [SerializeField] private float glbMoveSpeed = GLB_MOVE_SPEED;
    [SerializeField] private float glbJumpForce = GLB_JUMP_FORCE;
    [SerializeField] private float glbGravityScale = GLB_GRAVITY_SCALE;

    // 目標速度、終端速度みたいなイメージ
    [SerializeField] private float movementX;
    [SerializeField] private float movementY;

    [SerializeField] private float storedImpulseX;
    [SerializeField] private float storedImpulseY;

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
    public void AddImpulseVec(Vector2 addImpulse)
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            if (MathF.Abs(addImpulse.x) > MathF.Abs(storedImpulseX))
            {
                // 大きい方で上書き
                storedImpulseX = addImpulse.x;
            }
            else
            {
                // TODO: 分母の調整
                storedImpulseX += addImpulse.x / (1 + MathF.Abs(storedImpulseX));
            }
            
            if (MathF.Abs(addImpulse.y) > MathF.Abs(storedImpulseY))
            {
                // 大きい方で上書き
                storedImpulseY = addImpulse.y;
            }
            else
            {
                storedImpulseY += addImpulse.y / (1 + MathF.Abs(storedImpulseY));
            }
        }
        else
        {
            rb.linearVelocity = rb.linearVelocity / 2.5f + addImpulse;
        } 
    }

    public void AddImpulseX(float impulseX)
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            storedImpulseX += impulseX * glbMoveSpeed;
            return;
        }
        else
        {
            rb.linearVelocityX = rb.linearVelocityX / 2.5f + impulseX * glbMoveSpeed;
        }

    }

    public void AddImpulseY(float impulseY)
    {
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            storedImpulseY += impulseY * glbJumpForce;
            return;
        }
        else
        {
            rb.linearVelocityY = rb.linearVelocityY / 2.5f + impulseY * glbJumpForce;

        }
    }


    // * 移動の更新
    private void UpdateMovement()
    {
        if(rb.bodyType == RigidbodyType2D.Static) return;

        if (playerCore.jumpCount.Value >= 1)
        {
            // 空中にいるときはY速度を更新
            rb.linearVelocityX = rb.linearVelocityX + (movementX - rb.linearVelocityX) * 0.03f * Time.deltaTime * 60;
            rb.linearVelocityY = rb.linearVelocityY - glbGravityScale * Time.deltaTime;
        }
        else
        {
            // 地上にいるとき
            rb.linearVelocityX = rb.linearVelocityX + (movementX - rb.linearVelocityX) * 0.2f * Time.deltaTime * 60;
            rb.linearVelocityY = rb.linearVelocityY - glbGravityScale * Time.deltaTime;
        }
        
    }
}
