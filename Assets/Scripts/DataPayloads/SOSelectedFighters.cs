using UnityEngine;

[CreateAssetMenu(fileName = "SOSelectedFighters", menuName = "SelectedFighters SO")]
public class SOSelectedFighters : ScriptableObject
{
    public int[] SelectedFighterIDs;
    public Difficulty[] SelectedDifficulties;
    public float[] SelectedSpeeds;

    public float soundOffset;
}
