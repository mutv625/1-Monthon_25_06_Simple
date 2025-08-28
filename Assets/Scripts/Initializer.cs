using UnityEngine;

/// <summary>
/// DIとPresenterの役割を持つクラス
/// </summary>
public class Initializer : MonoBehaviour
{
    [SerializeField] public FightingEntryPoint fightingEntryPoint;

    public PlayerCore InitializePlayer(PlayerCore playerPrefab, int playerId, SOKeyConfig keyConfig, SOFighterPayload fighterPayload)
    {
        PlayerCore player = InstantiatePlayer(playerPrefab, playerId);
        InitializeInputProvider(player, keyConfig);
        InitializePlayerMover(player);
        InitializeGroundChecker(player);
        InitializePlayerAnimator(player, fighterPayload.AnimatorController);
        InitializeSkillController(player, fighterPayload);
        return player;
    }

    public PlayerCore InstantiatePlayer(PlayerCore playerPrefab, int playerId)
    {
        PlayerCore player = Instantiate(playerPrefab);
        player.SetPlayerId(playerId);
        player.Activate(fightingEntryPoint);
        return player;
    }

    public void InitializeInputProvider(PlayerCore player, SOKeyConfig keyConfig)
    {
        InputProvider inputProvider = player.gameObject.GetComponent<InputProvider>();
        inputProvider.SetKeyConfig(keyConfig);
        inputProvider.Activate(fightingEntryPoint);
    }

    public void InitializePlayerMover(PlayerCore player)
    {
        PlayerMover playerMover = player.gameObject.GetComponent<PlayerMover>();
        playerMover.Activate(fightingEntryPoint);
    }

    public void InitializeGroundChecker(PlayerCore player)
    {
        GroundChecker groundChecker = player.gameObject.transform.Find("GroundCheckers").GetComponent<GroundChecker>();
        groundChecker.Activate(fightingEntryPoint, player);
    }

    public void InitializePlayerAnimator(PlayerCore player, RuntimeAnimatorController animatorOverrideController)
    {
        PlayerAnimator playerAnimator = player.gameObject.GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(animatorOverrideController);
        playerAnimator.Activate(fightingEntryPoint);
    }

    public void InitializeSkillController(PlayerCore player, SOFighterPayload fighterPayload)
    {
        SkillController skillController = player.gameObject.GetComponent<SkillController>();
        skillController.SetPrefabsLists(fighterPayload);
        skillController.Activate();
    }
}
