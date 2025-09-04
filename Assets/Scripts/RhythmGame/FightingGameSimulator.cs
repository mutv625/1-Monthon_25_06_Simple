using System.Collections.Generic; // Listを使うために必要
using UnityEngine;

/// <summary>
/// RhythmGameManagerを外部から操作するためのテスト用クラス。
/// 格闘ゲームパートとの連携をシミュレートします。
/// </summary>
public class FightingGameSimulator : MonoBehaviour
{
    [Header("操作対象")]
    public RhythmGameManager rhythmGameManager; // シーンに配置されたRhythmGameManager

    [Header("テスト用アセット")]
    public AudioClip testIntroBgm; // イントロ用のBGM
    public TextAsset testIntroChart; // イントロ用の譜面ファイル(.txt)
    public AudioClip testLoopBgm;  // ループ用のBGM
    public TextAsset testLoopChart;  // ループ用の譜面ファイル(.txt)

    [Header("各プレイヤーの難易度設定")]
    [Tooltip("プレイヤーごとの難易度を設定（要素0=1P, 要素1=2P）")]
    public List<Difficulty> playerDifficulties = new List<Difficulty>();

    [Header("テスト設定")]
    public bool enableSimulator = true; // シミュレーターを有効にするかどうか
    [Tooltip("イントロ付きで再生をテストする場合はtrueにする")]
    public bool testWithIntro = false;
    [Tooltip("再生を開始したい時間（秒）")]
    public float testStartTime = 0f;

    void Awake()
    {
        // オーディオのデコードを事前に実行（CompressedInMemory 等だと再生時にデコードされてスパイクする）
        if (testIntroBgm != null)
        {
            testIntroBgm.LoadAudioData(); // 非同期読み込みは内部で行われるが、早めに呼ぶ
        }
        if (testLoopBgm != null)
        {
            testLoopBgm.LoadAudioData();
        }
    }

    void Update()
    {
        if (!enableSimulator || rhythmGameManager == null)
        {
            return; // シミュレーターが無効、または操作対象が設定されていない場合は何もしない
        }
        
        // "S"キーで、設定に基づいてリズムゲームを開始する
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (rhythmGameManager != null && testLoopBgm != null && testLoopChart != null)
            {
                if (testWithIntro)
                {
                    // イントロ付きで開始
                    Debug.Log($"【シミュレーター】: イントロ付きで、{testStartTime}秒からゲームを開始します。");
                    rhythmGameManager.StartRhythmGameWithIntro(testIntroBgm, testIntroChart, testLoopBgm, testLoopChart, testStartTime, playerDifficulties.ToArray());
                }
                else
                {
                    // 通常の開始
                    Debug.Log($"【シミュレーター】: {testStartTime}秒からゲームを開始します。");
                    rhythmGameManager.StartRhythmGame(testLoopBgm, testLoopChart, testStartTime, playerDifficulties.ToArray());
                }
            }
        }

        // "X"キーでゲームを停止
        if (Input.GetKeyDown(KeyCode.X))
        {
            rhythmGameManager.StopRhythmGame();
        }

        // "J"キーでP1の判定
        if (Input.GetKeyDown(KeyCode.J))
        {
            JudgeResult result = rhythmGameManager.Judge(0);
            Debug.Log("【シミュレーター】: プレイヤー1の判定結果は " + result);
        }

        // "K"キーでP2の判定
        if (Input.GetKeyDown(KeyCode.K))
        {
            JudgeResult result = rhythmGameManager.Judge(1);
            Debug.Log("【シミュレーター】: プレイヤー2の判定結果は " + result);
        }

        // "M"キーでP1のレーンを移動・回転
        if (Input.GetKeyDown(KeyCode.M))
        {
            rhythmGameManager.playerControllers[0].SetPose(new Vector2(-3, 2), 45f);
        }
    }
}