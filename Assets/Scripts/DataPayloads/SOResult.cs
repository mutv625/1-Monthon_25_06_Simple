using UnityEngine;

[CreateAssetMenu(fileName = "SOResult", menuName = "Result SO")]
public class SOResult : ScriptableObject
{
    public int winnerPlayerID = -1; // -1: 初期値

    public int[] maxComboCounts = new int[2];
    public int[] maxComboDamages = new int[2];


    public void Reset()
    {
        winnerPlayerID = -1;
        maxComboCounts[0] = 0;
        maxComboCounts[1] = 0;
        maxComboDamages[0] = 0;
        maxComboDamages[1] = 0;
    }
}
