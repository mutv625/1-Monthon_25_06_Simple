using UnityEngine;
using UniRx;

public class UIEntryPoint : MonoBehaviour
{
    [SerializeField] private FightingEntryPoint fightingEntryPoint;

    void Awake()
    {
        fightingEntryPoint.onFightingReady.Subscribe(players =>
        {
            // TODO: UIの初期化処理
        });
    }
}
