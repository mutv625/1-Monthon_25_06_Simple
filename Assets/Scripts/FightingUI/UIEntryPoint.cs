using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIEntryPoint : MonoBehaviour
{
    [SerializeField] private FightingEntryPoint fightingEntryPoint;

    [Header("体力バー")]
    [SerializeField] private HealthBarRenderer HealthBarP0;
    [SerializeField] private HealthBarRenderer HealthBarRedP0;
    [SerializeField] private HealthBarRenderer HealthBarP1;
    [SerializeField] private HealthBarRenderer HealthBarRedP1;

    [Header("体力数値")]
    [SerializeField] private HealthValueRenderer HealthValueP0;
    [SerializeField] private HealthValueRenderer HealthValueP1;

    [Header("コンボ表示")]
    [SerializeField] private ComboDisplayManager ComboDisplayP0;
    [SerializeField] private ComboDisplayManager ComboDisplayP1;

    void Awake()
    {
        fightingEntryPoint.onFightingReady.Subscribe(_ =>
        {
            if (fightingEntryPoint.players.Count < 2)
            {
                Debug.LogError("Not enough players to initialize UI.");
                return;
            }

            // * HealthBarの初期化
            HealthBarP0.Initialize(fightingEntryPoint.players[0]);
            HealthBarRedP0.Initialize(fightingEntryPoint.players[0]);
            HealthBarP1.Initialize(fightingEntryPoint.players[1]);
            HealthBarRedP1.Initialize(fightingEntryPoint.players[1]);

            // * HealthValueの初期化
            HealthValueP0.Initialize(fightingEntryPoint.players[0]);
            HealthValueP1.Initialize(fightingEntryPoint.players[1]);

            // * ComboDisplayの初期化
            ComboDisplayP0.Initialize(fightingEntryPoint.players[0]);
            ComboDisplayP1.Initialize(fightingEntryPoint.players[1]);

            // UI表示を順番に

            // TODO: 全部終わったらFightingEntryPointにゲーム開始を通知
        });
    }
}
