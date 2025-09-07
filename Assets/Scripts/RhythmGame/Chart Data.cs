using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChartData", menuName = "Rhythm Game/Chart Data")]
public class ChartData : ScriptableObject
{
    [Header("楽曲データ")]
    public AudioClip bgm;

    [Header("譜面データ")]
    public List<NoteData> notes = new List<NoteData>();
}