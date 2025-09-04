using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FightingEntryPoint : MonoBehaviour
{
    [Header("突入時データ")]
    [SerializeField] public SOSelectedFighters selectedFighters;

    [Header("=== デバッグ用 ===")]
    [SerializeField] private bool isRhythmSceneEnabled;


    [Header("戦闘シーン設定項目")]
    [SerializeField] private bool isTimeFlowing = true;

    [SerializeField] private List<PlayerCore> players = new List<PlayerCore>();
    [SerializeField] private List<SOKeyConfig> keyConfigs = new List<SOKeyConfig>();
    [SerializeField] private List<SOFighterPayload> fighterPayloads = new List<SOFighterPayload>();

    private Initializer initializer;

    [SerializeField] private PlayerCore playerPrefab;

    [Header("リズムゲームシーン関連")]
    [SerializeField] public AudioListener[] listeners;
    [SerializeField] PlayerController[] playerLanes;
    
    [SerializeField] public RhythmGameManager rhythmGameManager;

    // # 初期化処理

    /// <summary>
    /// 音ゲーパートのセットアップコルーチンが完了したときに発行されるイベント
    /// </summary>
    public Subject<Unit> onRhythmGameReady = new Subject<Unit>();
    /// <summary>
    /// 戦闘パートのセットアップが完了したときに発行されるイベント
    /// </summary>
    public Subject<Unit> onFightingReady = new Subject<Unit>();

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        initializer = GetComponent<Initializer>();
        initializer.fightingEntryPoint = this;


        // * 1. 音ゲーパートのセットアップ
        if (isRhythmSceneEnabled)
        {
            Debug.Log("EP >> 音ゲーシーンをセットアップします。");
            StartCoroutine(SetupRhythmGame());
        }


        // * 2. 戦闘パートのセットアップ
        SetupFighting(
            selectedFighters.SelectedFighterIDs[0],
            selectedFighters.SelectedFighterIDs[1]
        );
    }


    void TestStartGame()
    {
        // TODO: 音ゲーパートの曲開始
    }


    // # ゲームシーン初期化系

    private void SetupFighting(params int[] fighterIDs)
    {
        for (int i = 0; i < fighterIDs.Length; i++)
        {
            if (i >= keyConfigs.Count)
            {
                Debug.LogError("EP Fter >> KeyConfigs が足りません");
                break;
            }
            if (fighterIDs[i] >= fighterPayloads.Count)
            {
                Debug.LogError("EP Fter >> fighterID の指定が範囲外です");
                break;
            }

            players.Add(initializer.InitializePlayer(playerPrefab, i, keyConfigs[i], fighterPayloads[fighterIDs[i]]));
        }
    }

    private IEnumerator SetupRhythmGame()
    {
        Debug.Log("EP R >> 音ゲーシーンを非同期で読み込みます。");
        var ao = SceneManager.LoadSceneAsync("RhythmScene", LoadSceneMode.Additive);

        while (!ao.isDone)
        {
            yield return null;
        }
        Debug.Log($"EP R >> 音ゲーシーンの読み込みが完了しました。");

        // AudioListenerを一つ以外は無効化
        listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.InstanceID);
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                listeners[i].enabled = false;
            }
        }

        var rhythmGameManagers = FindObjectsByType<RhythmGameManager>(FindObjectsSortMode.None);
        if (rhythmGameManagers.Length == 0)
        {
            Debug.LogError("EP R >> RhythmGameManager が見つかりません。");
            yield break;
        }

        rhythmGameManager = rhythmGameManagers[0];

        // // 最初はレーンは非表示にしておく
        // playerLanes = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        // foreach (var lane in playerLanes)
        // {
        //     UIUtils.SetVisibleRecursively(lane.gameObject, false);
        // }

        onRhythmGameReady.OnNext(Unit.Default);
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

    // # 終了処理

    // TODO: 戦闘パートの終了処理
    // TODO: 音ゲーパートの終了処理
    // TODO: リザルト画面のセットアップ
}