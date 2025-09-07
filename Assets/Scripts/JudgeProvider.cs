using UnityEngine;

public class JudgeProvider : MonoBehaviour
{
    private FightingEntryPoint fightingEP;
    [SerializeField] private RhythmGameManager rhythmGameManager;

    [SerializeField] private JudgeResult debugJudgeResult;    

    public void Activate(FightingEntryPoint entryPoint)
    {
        fightingEP = entryPoint;
        rhythmGameManager = entryPoint.rhythmGameManager;
    }

    public JudgeResult Judge(int playerId)
    {
        if (fightingEP.isRhythmSceneEnabled == false)
        {
            Debug.LogWarning("JudgeProvider >> デバッグ用判定を返します");
            return debugJudgeResult;
        }
        else
        {
            if (rhythmGameManager == null)
            {
                Debug.LogError("JudgeProvider >> RhythmGameManager 無しで判定を返します");
                return debugJudgeResult;
            }
            else
            {
                Debug.Log("JudgeProvider >> RhythmGameManager から判定を取得します");
                return rhythmGameManager.Judge(playerId);
            }
        }
    }


}
