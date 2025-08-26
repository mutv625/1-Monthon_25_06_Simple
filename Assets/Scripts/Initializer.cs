using UnityEngine;

/// <summary>
/// DIとPresenterの役割を持つクラス
/// </summary>
public class Initializer : MonoBehaviour
{
    public PlayerCore InstantiatePlayer(PlayerCore playerPrefab, FightingEntryPoint fightingEntryPoint, int playerId)
    {
        PlayerCore player = Instantiate(playerPrefab);
        player.SetPlayerId(playerId);
        player.Activate(fightingEntryPoint);
        return player;
    }

    public void InitializeInputProvider(PlayerCore player, FightingEntryPoint fightingEntryPoint, SOKeyConfig keyConfig)
    {
        InputProvider inputProvider = player.gameObject.GetComponent<InputProvider>();
        inputProvider.SetKeyConfig(keyConfig);
        inputProvider.Activate(fightingEntryPoint);
    }

    public void InitializePlayerMover(PlayerCore player, FightingEntryPoint fightingEntryPoint)
    {
        PlayerMover playerMover = player.gameObject.GetComponent<PlayerMover>();
        playerMover.Activate(fightingEntryPoint);
    }

    public void InitializePlayerAnimator(PlayerCore player, FightingEntryPoint fightingEntryPoint, RuntimeAnimatorController animatorOverrideController)
    {
        PlayerAnimator playerAnimator = player.gameObject.GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(animatorOverrideController);
        playerAnimator.Activate(fightingEntryPoint);
    }

    public void InitializeSkillController(PlayerCore player, FightingEntryPoint fightingEntryPoint, SOFighterPayload fighterPayload)
    {
        SkillController skillController = player.gameObject.GetComponent<SkillController>();
        skillController.SetPrefabsLists(fighterPayload);
        skillController.Activate();
    }
}
