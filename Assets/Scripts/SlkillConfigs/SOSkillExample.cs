using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSOSkillExample", menuName = "Skill SO/SOSkillExample")]
public class SOSkillExample : SOSkill
{
    [SerializeField] string skillName = "Example Skill";
    public override string SkillName => skillName;

    [SerializeField] GameObject hitboxPrefab;

    public override async void ExecuteSkill(PlayerCore executor)
    {
        GameObject hitboxInstance = Instantiate(hitboxPrefab, executor.transform);
        await Task.Delay(1000); // Simulate skill execution delay
        Destroy(hitboxInstance);
    }

}
