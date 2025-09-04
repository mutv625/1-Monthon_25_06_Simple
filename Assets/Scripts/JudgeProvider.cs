using UnityEngine;

public class JudgeProvider : MonoBehaviour
{
    private FightingEntryPoint fightingEntryPoint;
    private RhythmGameManager rhythmGameManager;

    [SerializeField] private JudgeResult debugJudgeResult;    

    public void Activate(FightingEntryPoint entryPoint)
    {
        fightingEntryPoint = entryPoint;
        rhythmGameManager = entryPoint.rhythmGameManager;
    }

    public JudgeResult Judge(int playerId)
    {
        if (rhythmGameManager == null)
        {
            Debug.LogWarning("JudgeProvider >> RhythmGameManager 無しで判定を返します");
            return debugJudgeResult;
        }
        else
        {
            return rhythmGameManager.Judge(playerId);
        }
    }


}
