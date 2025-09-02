using UnityEngine;
using UniRx;

public class InputProvider : MonoBehaviour
{
    [SerializeField] private SOKeyConfig keyConfig;

    private PlayerCore playerCore;

    public void SetKeyConfig(SOKeyConfig keyConfig)
    {
        this.keyConfig = keyConfig;
        playerCore = GetComponent<PlayerCore>();

        Debug.Log($"InputProvider initialized for Player ID: {playerCore.PlayerId}");
    }

    public void Activate(FightingEntryPoint fightingEntryPoint)
    {
        fightingEntryPoint.updateInFighting
            .Subscribe(_ => UpdateKeyInput())
            .AddTo(this);
    }

    public void UpdateKeyInput()
    {
        float inputX = 0;
        if (Input.GetKey(keyConfig.moveLeftKey))
        {
            inputX += -1;
        }
        if (Input.GetKey(keyConfig.moveRightKey))
        {
            inputX += 1;
        }
        playerCore.Move(inputX);

        float inputY = 0;
        if (Input.GetKeyDown(keyConfig.moveUpKey))
        {
            playerCore.Jump();
        }
        if (Input.GetKey(keyConfig.moveDownKey))
        {
            inputY += -1;
        }
        playerCore.Fall(inputY);

        if (Input.GetKeyDown(keyConfig.skillAKey))
        {
            playerCore.SkillA();
        }
        if (Input.GetKeyDown(keyConfig.skillBKey))
        {
            playerCore.SkillB();
        }
    }

}
