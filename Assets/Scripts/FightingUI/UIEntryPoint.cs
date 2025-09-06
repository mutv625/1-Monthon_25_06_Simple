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

    [Header("リズムパートUI")]
    [SerializeField] private PlayerCore[] players;
    [SerializeField] private PlayerController[] playerLanes = new PlayerController[2];

    [SerializeField] private float laneOffsetX = 1.5f;
    [SerializeField] private float internalRatio = 0.5f;

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

            // * PlayerCoreの参照を保存
            players = fightingEntryPoint.players.ToArray();

        }).AddTo(this);

        fightingEntryPoint.onRhythmGameReady.Subscribe(_ =>
        {
            // * リズムパートのPlayerControllerの位置を反映するための初期化
            playerLanes = fightingEntryPoint.playerLanes;
            fightingEntryPoint.updateInFighting
                .Subscribe(_ => MovePlayerLanes(playerLanes))
                .AddTo(this);

            // * PlayerControllerのノーツ速度の設定
            for (int i = 0; i < playerLanes.Length; i++)
            {
                    playerLanes[i].noteSpeed = fightingEntryPoint.selectedFighters.SelectedSpeeds[i];
            }

        }).AddTo(this);
    }

    void MovePlayerLanes(params PlayerController[] playerControllers)
    {
        for (int i = 0; i < playerControllers.Length; i++)
        {
            var lane = playerControllers[i];
            if (lane != null && players[i] != null)
            {
                // 位置と回転をPlayerControllerに追従させる
                Vector2 targetPos;

                // -> x+    0P / 1P
                if (players[i].PlayerId % 2 == 0)
                {
                    targetPos = new Vector2(players[i].transform.position.x - 1.5f, players[i].transform.position.y);
                }
                else
                {
                    targetPos = new Vector2(players[i].transform.position.x + 1.5f, players[i].transform.position.y);
                }

                Vector2 toMove = targetPos - (Vector2)lane.transform.position;

                Vector2 goalPos = (Vector2)lane.transform.position + toMove * internalRatio * Time.deltaTime / 0.2f;


                lane.SetPose(goalPos, players[i].PlayerId % 2 == 0 ? 90f : -90f);


            }
        }
    }
}
