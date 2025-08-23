using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.ComponentModel;

public class FightingEntryPoint : MonoBehaviour
{
    [SerializeField] private bool isFighting = true;

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
    }


    void Start()
    {
        players.Add(initializer.InstantiatePlayer(playerPrefab, 0));
        initializer.InitializeInputProvider(players[0], this, keyConfigs[0]);
        initializer.InitializePlayerMover(players[0], this);
        initializer.InitializePlayerAnimator(players[0], this, fighterPayloads[0].AnimatorController);
    }


    public Subject<Unit> onFightingStart = new Subject<Unit>();
    public Subject<Unit> updateAlways = new Subject<Unit>();
    public Subject<Unit> updateInFighting = new Subject<Unit>();

    void Update()
    {
        updateAlways.OnNext(Unit.Default);
        if (isFighting) updateInFighting.OnNext(Unit.Default);
    }
}