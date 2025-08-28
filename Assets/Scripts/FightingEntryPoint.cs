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
        initializer.fightingEntryPoint = this;
    }


    void Start()
    {
        players.Add(initializer.InstantiatePlayer(playerPrefab, 0));
        initializer.InitializeInputProvider(players[0], keyConfigs[0]);
        initializer.InitializePlayerMover(players[0]);
        initializer.InitializeGroundChecker(players[0]);
        initializer.InitializePlayerAnimator(players[0], fighterPayloads[0].AnimatorController);
        initializer.InitializeSkillController(players[0], fighterPayloads[0]);
        

        players.Add(initializer.InitializePlayer(playerPrefab, 1, keyConfigs[1], fighterPayloads[0]));
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