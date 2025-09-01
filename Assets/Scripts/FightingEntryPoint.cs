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
        // * 0P 初期化
        players.Add(initializer.InitializePlayer(playerPrefab, 0, keyConfigs[0], fighterPayloads[0]));

        // * 1P 初期化
        players.Add(initializer.InitializePlayer(playerPrefab, 1, keyConfigs[1], fighterPayloads[0]));
    }


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
                    player.comboState.Value = ComboStates.None;
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