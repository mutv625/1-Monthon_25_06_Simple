using UnityEngine;
using UniRx;

public class SkillExecutor : MonoBehaviour
{
    [SerializeField] private SOSkill skillA;
    [SerializeField] private SOSkill skillB;

    public void SetupSkills(params SOSkill[] skills)
    {
        skillA = ScriptableObject.Instantiate(skills[0]);
        skillA.InitializeSkill();

        skillB = skills[1] ? ScriptableObject.Instantiate(skills[1]) : null;
        skillB?.InitializeSkill();
    }

    PlayerCore playerCore;

    public void Activate()
    {
        playerCore = GetComponent<PlayerCore>();
        
        playerCore.onSkillA
            .Subscribe(_ => skillA.Execute(playerCore))
            .AddTo(this);

        playerCore.onSkillB
            .Subscribe(_ => skillB.Execute(playerCore))
            .AddTo(this);
    }
}
