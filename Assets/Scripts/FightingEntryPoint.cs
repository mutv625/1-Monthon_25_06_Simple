using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.ComponentModel;

public class FightingEntryPoint : MonoBehaviour
{
    public Subject<Unit> onFightingStart = new Subject<Unit>();

    public Subject<Unit> updateAlways = new Subject<Unit>();

    [SerializeField] private bool isFighting = true;
    public Subject<Unit> updateInFighting = new Subject<Unit>();


    [SerializeField] private List<PlayerCore> players = new List<PlayerCore>();
    [SerializeField] private List<SOKeyConfig> keyConfigs = new List<SOKeyConfig>();

    Initializer initializer;

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

        initializer.InitializeSkillExecutorDebug(players[0]);
    }


    void Update()
    {
        updateAlways.OnNext(Unit.Default);
        if (isFighting) updateInFighting.OnNext(Unit.Default);
    }
}