using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.ComponentModel;

public class FightingEntryPoint : MonoBehaviour
{
    [SerializeField] private bool isTimeFlowing = true;

    [SerializeField] private List<PlayerCore> players = new List<PlayerCore>();
    [SerializeField] private List<SOKeyConfig> keyConfigs = new List<SOKeyConfig>();
    [SerializeField] private List<SOFighterPayload> fighterPayloads = new List<SOFighterPayload>();

    private Initializer initializer;

    [SerializeField] private PlayerCore playerPrefab;


    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        initializer = GetComponent<Initializer>();
        initializer.fightingEntryPoint = this;
    }


    void Start()
    {
        // TODO 1. 音ゲーパートのセットアップ

        // * 2. 戦闘パートのセットアップ
        SetupFighting(0, 0);
    }

    // TODO 3. 戦闘パートの終了処理
    // TODO 4. 音ゲーパートの終了処理
    // TODO 5. リザルト画面のセットアップ

    // # ゲームシーン初期化系

    private void SetupFighting(params int[] fighterIDs)
    {
        for (int i = 0; i < fighterIDs.Length; i++)
        {
            if (i >= keyConfigs.Count)
            {
                Debug.LogError("キーコンフィグが足りません");
                break;
            }
            if (fighterIDs[i] >= fighterPayloads.Count)
            {
                Debug.LogError("ファイターペイロードの指定が範囲外です");
                break;
            }

            players.Add(initializer.InitializePlayer(playerPrefab, i, keyConfigs[i], fighterPayloads[fighterIDs[i]]));
        }
    }


    // # 購読可能イベント
    public Subject<Unit> onFightingStart = new Subject<Unit>();
    public Subject<Unit> updateAlways = new Subject<Unit>();
    public Subject<Unit> updateInFighting = new Subject<Unit>();

    void Update()
    {
        updateAlways.OnNext(Unit.Default);
        if (isTimeFlowing) updateInFighting.OnNext(Unit.Default);
    }

    public void FinishComboForEveryone()
    {
        foreach (var player in players)
        {
            switch (player.comboState.Value)
            {
                case ComboStates.Trapped:
                    player.comboState.Value = ComboStates.Ending;
                    break;
                case ComboStates.None:
                    break;
                case ComboStates.Combo:
                    player.comboState.Value = ComboStates.Ending;
                    break;
                case ComboStates.Ending:
                    break;
            }
        }
    }
}