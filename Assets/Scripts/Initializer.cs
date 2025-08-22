using UnityEngine;

/// <summary>
/// DIとPresenterの役割を持つクラス
/// </summary>
public class Initializer : MonoBehaviour
{
    public PlayerCore InstantiatePlayer(PlayerCore playerPrefab, int playerId)
    {
        PlayerCore player = Instantiate(playerPrefab);
        player.SetPlayerId(playerId);
        return player;
    }

    public void InitializeInputProvider(PlayerCore player, FightingEntryPoint fightingEntryPoint, SOKeyConfig keyConfig)
    {
        InputProvider inputProvider = player.gameObject.AddComponent<InputProvider>();
        inputProvider.SetKeyConfig(keyConfig);
        inputProvider.Activate(fightingEntryPoint);
    }

    public void InitializePlayerMover(PlayerCore player, FightingEntryPoint fightingEntryPoint)
    {
        PlayerMover playerMover = player.gameObject.AddComponent<PlayerMover>();
        playerMover.Activate(fightingEntryPoint);
    }

    public void InitializeSkillExecutor(PlayerCore player, SOSkill[] skills)
    {
        SkillExecutor skillExecutor = player.gameObject.AddComponent<SkillExecutor>();
        skillExecutor.SetupSkills(skills);
        skillExecutor.Activate();
    }

    public void InitializeSkillExecutorDebug(PlayerCore player)
    {
        SkillExecutor skillExecutor = player.gameObject.AddComponent<SkillExecutor>();
        skillExecutor.Activate();
    }

}
