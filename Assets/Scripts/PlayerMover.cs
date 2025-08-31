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
        playerCore.onHurtAndKB
            .Subscribe(kbVec =>
            {
                // if (playerCore.comboState.Value == ComboStates.Combo)
                // {
                //     // コンボ中は物理演算を停止し、ノックバックを1/3にする
                //     rb.simulated = false;
                //     AddImpulseVec(kbVec / 3);
                // }
                // else
                // {
                //     rb.simulated = true;
                //     AddImpulseVec(kbVec);
                // }

                rb.simulated = true;
                AddImpulseVec(kbVec);
            });
    }

    // * 移動用定数
    // ゲーム内共通の値、キャラクターごとのものではない
    [SerializeField] private readonly float MOVE_SPEED = 10f;
    [SerializeField] private readonly float JUMP_FORCE = 8f;
    [SerializeField] private readonly float GRAVITY_SCALE = 13f;

    // 目標速度、終端速度みたいなイメージ
    [SerializeField] private float movementX;
    [SerializeField] private float movementY;

    // * 移動系
    public void MoveX(float inputX)
    {
        movementX = inputX * MOVE_SPEED;
    }

    public void MoveY(float inputY)
    {
        movementY = inputY * MOVE_SPEED;
    }

    // * 力を加える系
    public void AddImpulseVec(Vector2 impulse)
    {
        Debug.Log($"AddImpulseVec: {impulse}");
        rb.linearVelocity = rb.linearVelocity / 2 + impulse;
    }

    public void AddImpulseY(float impulseY)
    {
        rb.linearVelocityY = rb.linearVelocityY / 2 + impulseY * JUMP_FORCE;
    }


    // * 移動の更新
    private void UpdateMovement()
    {
        rb.linearVelocityX = rb.linearVelocityX + (movementX - rb.linearVelocityX) * 0.2f * Time.deltaTime * 60;
        rb.linearVelocityY = rb.linearVelocityY - GRAVITY_SCALE * Time.deltaTime;
    }
}
