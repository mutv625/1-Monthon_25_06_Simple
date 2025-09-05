using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リズムゲーム全体の進行を管理する司令塔クラス
/// 格闘ゲームパートなど、外部のスクリプトはこのクラスのpublicメソッドを介して音ゲーパートを操作
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RhythmGameManager : MonoBehaviour
{
    // === Inspectorから設定する項目 ===
    [Header("オブジェクト設定")]
    public GameObject normalNotePrefab; // 通常ノーツとして生成されるプレハブ
    public GameObject largeNotePrefab;  // 大ノーツとして生成されるプレハブ
    public PlayerController[] playerControllers;    // 各プレイヤーのコントローラー（PlayerControllerを持つGameObject）をここに設定

    [Header("タイミング設定")]
    [Tooltip("全体のノーツタイミングを調整します。プラスの値で遅く、マイナスの値で早くなります。")]
    public float timingOffset = 0f;

    [Header("判定設定 (時間)")]
    public float perfectThreshold = 0.1f;   // Perfectの判定幅(秒)
    public float goodThreshold = 0.2f;  // Goodの判定幅(秒)

    [Header("デバッグ設定")]
    public bool isDebugMode = false;    // trueにするとデバック用のコンソールを表示

    // === プライベート変数 ===
    private AudioSource audioSource;
    private List<NoteData>[] playerNotes;   // 各プレイヤーの譜面データ
    private int[] playerSpawnIndexes;       // 各プレイヤーのノーツ生成進捗
    private bool isPlaying = false;         // ゲームがプレイ中かどうかのフラグ
    private float[] playerNoteTravelTimes;  // 各プレイヤーのノーツ移動時間

    // イントロ・ループ再生用の変数
    private AudioClip introClip;
    private AudioClip loopClip;
    private bool hasIntro = false;
    private float totalPlaybackTime = 0f;





    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // プレイヤー数に合わせて各種配列を初期化
        playerNotes = new List<NoteData>[playerControllers.Length];
        playerSpawnIndexes = new int[playerControllers.Length];
        playerNoteTravelTimes = new float[playerControllers.Length];
    }

    void Update()
    {
        if (!isPlaying) return;

        // ゲーム開始からの総再生時間を独自に更新し続ける
        totalPlaybackTime += Time.deltaTime;

        // --- ループ遷移処理 ---
        // イントロがあり、イントロクリップが再生終了したらループクリップに切り替えて再生する
        if (hasIntro && audioSource.clip == introClip && !audioSource.isPlaying)
        {
            audioSource.clip = loopClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        // --- プレイヤーごとの処理 ---
        for (int i = 0; i < playerControllers.Length; i++)
        {
            // イントロを考慮した、譜面上の正しい現在再生時間を取得
            float currentPlaybackTime = GetCurrentPlaybackTime();
                
            // --- ノーツ生成処理 ---
            if (playerNotes[i] != null && playerSpawnIndexes[i] < playerNotes[i].Count)
            {
                float spawnTime = playerNotes[i][playerSpawnIndexes[i]].timing - playerNoteTravelTimes[i] + timingOffset;
                if (totalPlaybackTime >= spawnTime)
                {
                    SpawnNoteForPlayer(i, playerNotes[i][playerSpawnIndexes[i]]);
                    playerSpawnIndexes[i]++;
                }
            }

            // --- 見逃しMissの判定処理 ---
            if (playerControllers[i].activeNotes.Count > 0)
            {
                GameObject oldestNoteObject = playerControllers[i].activeNotes[0];
                NoteController oldestNote = oldestNoteObject.GetComponent<NoteController>();

                if (totalPlaybackTime > oldestNote.judgeTime + goodThreshold)
                {
                    Debug.Log($"Player {i} Missed! (見逃し)");
                    
                    oldestNote.OnMiss();
                    playerControllers[i].activeNotes.RemoveAt(0);
                }
            }
        }
    }
    

    // === 以下、外部から呼び出すためのAPI（publicメソッド） ===

    /// <summary>
    /// BGMをイントロとループに分けてリズムゲームを開始する
    /// </summary>
    /// <param name="intro">イントロのAudioClip</param>
    /// <param name="introChart">イントロの譜面ファイル</param>
    /// <param name="loop">ループのAudioClip</param>
    /// <param name="loopChart">ループの譜面ファイル</param>
    /// <param name="startTime">再生を開始したい時間（秒）</param>
    /// <param name="difficulties">各プレイヤーの難易度</param>
    public void StartRhythmGameWithIntro(AudioClip intro, TextAsset introChart, AudioClip loop, TextAsset loopChart, float startTime = 0f, params Difficulty[] difficulties)
    {
        this.introClip = intro;
        this.loopClip = loop;
        this.hasIntro = (intro != null);

        SetupGameInternal(introChart, loopChart, startTime, difficulties);

        float introDuration = hasIntro ? intro.length : 0f;
        if (hasIntro && startTime < introDuration)
        {
            audioSource.clip = introClip;
            audioSource.loop = false;
            audioSource.time = startTime;

        }
        else
        {
            audioSource.clip = loopClip;
            audioSource.loop = true;
            float loopStartTime = hasIntro ? startTime - introDuration : startTime;
            audioSource.time = Mathf.Max(0, loopStartTime);

        }

        totalPlaybackTime = startTime;
        audioSource.Play();
        isPlaying = true;
    }

    /// <summary>
    /// BGMを1曲通し（ループのみ）でリズムゲームを開始する
    /// </summary>
    /// <param name="bgm">再生するBGM</param>
    /// <param name="chartFile">解析する譜面ファイル</param>
    /// <param name="startTime">再生を開始したい時間（秒）</param>
    /// <param name="difficulties">各プレイヤーの難易度</param>
    public void StartRhythmGame(AudioClip bgm, TextAsset chartFile, float startTime = 0f, params Difficulty[] difficulties)
    {
        this.introClip = null;
        this.loopClip = bgm;
        this.hasIntro = false;
        
        SetupGameInternal(null, chartFile, startTime, difficulties);

        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.time = startTime;
        totalPlaybackTime = startTime;
        audioSource.Play();
        isPlaying = true;
    }

    /// <summary>
    /// リズムゲームを強制的に停止する
    /// </summary>
    public void StopRhythmGame()
    {
        audioSource.Stop();
        isPlaying = false;
    }

    /// <summary>
    /// 指定されたプレイヤーの判定を行う
    /// </summary>
    /// <param name="playerID">判定するプレイヤーID (0=1P, 1=2P)</param>
    /// <returns>判定結果</returns>    
    public JudgeResult Judge(int playerID)
    {
        if (playerID < 0 || playerID >= playerControllers.Length) return JudgeResult.None;
        PlayerController targetPlayer = playerControllers[playerID];

        // 判定対象のノーツがない場合はNoneを返す
        if (!isPlaying || targetPlayer.activeNotes.Count == 0)
        {
            return JudgeResult.None;
        }

        GameObject targetNoteObject = targetPlayer.activeNotes[0];
        NoteController targetNoteController = targetNoteObject.GetComponent<NoteController>();

        float timeDifference = Mathf.Abs(totalPlaybackTime - targetNoteController.judgeTime);

        // 判定許容範囲から外れていればNoneを返す
        if (timeDifference > goodThreshold)
        {
            return JudgeResult.None;
        }

        // 判定後、リストから削除し、ノーツ自身に破壊を命令
        targetPlayer.activeNotes.RemoveAt(0);
        targetNoteController.OnHit();

        // ノーツの種類に応じて結果を分岐
        NoteType noteType = targetNoteController.type;
        if (noteType == NoteType.Large)
        {
            return JudgeResult.Critical;
        }
        else
        {
            return timeDifference <= perfectThreshold ? JudgeResult.Perfect : JudgeResult.Good;
        }
    }


    // === 以下、内部処理用のprivateメソッド ===

    // ゲームのセットアップを行う共通の内部メソッド
    private void SetupGameInternal(TextAsset introChart, TextAsset loopChart, float startTime, params Difficulty[] difficulties)
    {
        isPlaying = false;
        // 古いノーツを全て削除
        foreach (var player in playerControllers)
        {
            foreach (var note in player.activeNotes) Destroy(note);
            player.activeNotes.Clear();
        }

        CalculateAllPlayerNoteTravelTimes();

        float introDuration = (hasIntro && introClip != null) ? introClip.length : 0f;
        float loopDuration = (loopClip != null) ? loopClip.length : 0f;

        // プレイヤーごとに譜面を準備
        for (int i = 0; i < playerControllers.Length; i++)
        {
            Difficulty difficulty = (i < difficulties.Length) ? difficulties[i] : Difficulty.EZ;

            List<NoteData> combinedNotes = new List<NoteData>();
            if (hasIntro && introChart != null)
            {
                combinedNotes.AddRange(ChartParser.Parse(introChart, difficulty));
            }

            if (loopChart != null && loopDuration > 0)
            {
                List<NoteData> baseLoopNotes = ChartParser.Parse(loopChart, difficulty);

                // 譜面を10周分用意 さらに増やす場合には loopCount < 10 の値を変更
                for (int loopCount = 0; loopCount < 10; loopCount++)
                {
                    foreach (var baseNote in baseLoopNotes)
                    {
                        // 新しいノーツデータとしてコピーを作成
                        NoteData loopedNote = new NoteData();
                        loopedNote.type = baseNote.type;
                        // ループ回数に応じて、BGMの長さ分だけタイミングを後ろにずらす
                        loopedNote.timing = baseNote.timing + (introDuration + (loopDuration * loopCount));

                        combinedNotes.Add(loopedNote);
                    }
                }
            }

            combinedNotes.Sort((a, b) => a.timing.CompareTo(b.timing));
            playerNotes[i] = combinedNotes;
            playerSpawnIndexes[i] = 0;

            // 途中再生のための事前生成処理
            for (int j = 0; j < playerNotes[i].Count; j++)
            {
                float spawnTime = playerNotes[i][j].timing - playerNoteTravelTimes[i] + timingOffset;
                if (spawnTime < startTime)
                {
                    playerSpawnIndexes[i] = j + 1;

                    float judgeTime = playerNotes[i][j].timing + timingOffset;
                    if (judgeTime > startTime)
                    {
                        float timeSinceSpawn = startTime - spawnTime;
                        float distanceTravelled = timeSinceSpawn * playerControllers[i].noteSpeed;
                        SpawnNoteForPlayer(i, playerNotes[i][j], distanceTravelled);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
    


    // 現在の譜面上の再生時間を取得する
    private float GetCurrentPlaybackTime()
    {
        if (!audioSource.isPlaying) return 0f;

        if (hasIntro && !audioSource.loop)
        {
            // イントロ再生中
            return audioSource.time;
        }
        else
        {
            // ループ部分再生中
            float introLength = hasIntro ? introClip.length : 0f;
            return introLength + audioSource.time;
        }
    }


    // 指定されたプレイヤーにノーツを1つ生成する
    private void SpawnNoteForPlayer(int playerID, NoteData noteData, float initialDistance = 0f)
    {
        PlayerController player = playerControllers[playerID];
        GameObject prefabToSpawn = (noteData.type == NoteType.Large) ? largeNotePrefab : normalNotePrefab;
        Transform spawnPoint = player.transform.Find("SpawnPoint");

        if (prefabToSpawn != null && spawnPoint != null)
        {
            GameObject newNoteObject = Instantiate(prefabToSpawn, spawnPoint);
            NoteController newNoteController = newNoteObject.GetComponent<NoteController>();
            
            newNoteController.speed = player.noteSpeed;
            newNoteController.judgeTime = noteData.timing + timingOffset;
            newNoteController.type = noteData.type;

            newNoteController.isDebug = this.isDebugMode;

            if (initialDistance > 0)
            {
                newNoteObject.transform.localPosition += Vector3.down * initialDistance;
            }
            
            player.activeNotes.Add(newNoteObject);
        }
    }
    
    // 全プレイヤーのノーツ移動時間を計算する
    private void CalculateAllPlayerNoteTravelTimes()
    {
        for (int i = 0; i < playerControllers.Length; i++)
        {
            PlayerController player = playerControllers[i];
            if (player == null) continue;

            Transform spawnPoint = player.transform.Find("SpawnPoint");
            Transform judgementLine = player.transform.Find("JudgementLine");

            if (spawnPoint != null && judgementLine != null)
            {
                float distance = Vector3.Distance(spawnPoint.position, judgementLine.position);
                if (player.noteSpeed > 0)
                {
                    playerNoteTravelTimes[i] = distance / player.noteSpeed;
                }
                else
                {
                    playerNoteTravelTimes[i] = 0;
                }
            }
        }
    }
}