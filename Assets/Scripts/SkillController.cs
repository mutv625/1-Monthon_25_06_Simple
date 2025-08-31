using System;
using UnityEngine;
using UniRx;

public class SkillController : MonoBehaviour
{
    private PlayerCore playerCore;

    [SerializeField] private HitboxPrefab[] hitboxPrefabs;
    [SerializeField] private EffectPrefab[] effectPrefabs;
    [SerializeField] private Action<int>[] triggerDelegates;

    [Header("閲覧専用 (ダメージ計算に使用)")]
    [SerializeField] private JudgeResult currentJudgeResult;

    // * SOFighterPayloadの積み込み
    public void SetPrefabsLists(SOFighterPayload fighterPayload)
    {
        hitboxPrefabs = fighterPayload.HitboxPrefabs;
        effectPrefabs = fighterPayload.EffectPrefabs;
        triggerDelegates = fighterPayload.TriggerDelegates;
    }

    public void Activate()
    {
        playerCore = gameObject.GetComponent<PlayerCore>();

        // * ダメージ計算用の判定を持っておけるように
        playerCore.onSkill
            .Subscribe(state => currentJudgeResult = state.Item2);
    }

    // * 以下の関数は AnimationClip の Animation Event から呼び出す

    public void GenerateHitbox(int index)
    {
        // ! 生成は必ずプレイヤーの *子オブジェクトとして* 、原点に、回転はゼロで生成する

        HitboxPrefab hitbox = Instantiate(hitboxPrefabs[index], playerCore.transform);
        hitbox.SetStatus(playerCore, currentJudgeResult);
    }

    public void GenerateEffect(int index)
    {
        // ! 生成は必ずプレイヤーの *子オブジェクトとして* 、原点に、回転はゼロで生成する

        EffectPrefab effect = Instantiate(effectPrefabs[index], playerCore.transform);
    }

    public void TriggerAction(int index)
    {
        triggerDelegates[index].Invoke(index);
    }

    public void ResetLockingStatus()
    {
        playerCore.ResetLockingStatusAtCore();
    }
}
