// この行を追加することで、このクラスのデータをUnityエディタ上で表示・保存できるようになります
[System.Serializable]
public class NoteData
{
    public float timing;   // ノーツのタイミング（秒）
    public NoteType type;  // ノーツの種類 (Normal or Large)
}