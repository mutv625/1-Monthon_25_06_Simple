using UnityEngine;

[CreateAssetMenu(fileName = "SOSelectedFighters", menuName = "SelectedFighters SO", order = 1)]
public class SOSelectedFighters : ScriptableObject
{
    public int[] SelectedFighterIDs;
    public Difficulty[] SelectedDifficulties;
    public float[] SelectedSpeeds;

    public float soundOffset;
}
