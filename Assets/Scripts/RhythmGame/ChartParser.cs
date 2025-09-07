using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

/// <summary>
/// Margreteからエクスポートされた譜面テキストファイル(.ugc.txt)を解析する静的クラス。
/// GameObjectにアタッチする必要はありません。
/// </summary>
public static class ChartParser
{
    /// <summary>
    /// 譜面テキストファイルを解析し、NoteDataのリストに変換する
    /// </summary>
    /// <param name="chartFile">譜面テキストファイル</param>
    /// <param name="difficulty">解析したい難易度</param>
    /// <returns>ノーツデータのリスト</returns>
    public static List<NoteData> Parse(TextAsset chartFile, Difficulty difficulty)
    {
        var notes = new List<NoteData>();
        if (chartFile == null) return notes;

        // --- ヘッダー情報の解析 ---
        float bpm = 120.0f;
        int ticksPerBeat = 480;
        int beatsPerBar = 4;
        bool inNoteSection = false;

        string[] lines = chartFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (line.StartsWith("@ENDHEAD"))
            {
                inNoteSection = true;
                continue;
            }

            if (!inNoteSection)
            {
                string[] columns = line.Split(new[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (columns.Length < 2) continue;
                try
                {
                    string key = columns[0];
                    if (key == "@BPM") bpm = float.Parse(columns[2], CultureInfo.InvariantCulture);
                    if (key == "@TICKS") ticksPerBeat = int.Parse(columns[1]);
                    if (key == "@BEAT" && columns.Length > 2) beatsPerBar = int.Parse(columns[2]);
                }
                catch { /* パース失敗は無視 */ }
            }
            else
            {
                // --- ノーツ解析 ---
                if (!line.StartsWith("#")) continue;
                try
                {
                    string[] mainParts = line.Substring(1).Split(':');
                    string[] timeParts = mainParts[0].Split('\'');
                    string noteInfo = mainParts[1];

                    // 小節番号をそのまま使う
                    int bar = int.Parse(timeParts[0]);
                    int tick = int.Parse(timeParts[1]);

                    // レーン番号を16進数から10進数に変換
                    int lane = int.Parse(noteInfo.Substring(1, 1), NumberStyles.HexNumber);

                    // 難易度に応じて、通常/大ノーツのレーン番号を決定
                    int normalLane, largeLane;
                    switch (difficulty)
                    {
                        case Difficulty.EZ: normalLane = 0; largeLane = 2; break;
                        case Difficulty.HD: normalLane = 6; largeLane = 8; break;
                        case Difficulty.IN: normalLane = 12; largeLane = 14; break;
                        default: continue;
                    }

                    NoteType noteType;
                    if (lane == normalLane) noteType = NoteType.Normal;
                    else if (lane == largeLane) noteType = NoteType.Large;
                    else continue;

                    // 時間計算 (小節/tick -> 秒)
                    double secondsPerBeat = 60.0 / bpm;
                    double totalBeats = (double)(bar * beatsPerBar) + ((double)tick / ticksPerBeat);
                    float timing = (float)(totalBeats * secondsPerBeat);

                    notes.Add(new NoteData { timing = timing, type = noteType });
                }
                catch (System.Exception e)
                {
                    Debug.LogError("譜面行の解析に失敗: " + line + " | " + e.Message);
                }
            }
        }

        // 最終的なノーツリストを時間順にソート
        notes.Sort((a, b) => a.timing.CompareTo(b.timing));
        return notes;
    }
}