using UnityEngine;

[CreateAssetMenu(fileName = "KeyConfig", menuName = "KeyConfig SO")]
public class SOKeyConfig : ScriptableObject
{
    [SerializeField] public KeyCode moveLeftKey;
    [SerializeField] public KeyCode moveRightKey;
    [SerializeField] public KeyCode moveUpKey;
    [SerializeField] public KeyCode moveDownKey;

    [SerializeField] public KeyCode skillAKey;
    [SerializeField] public KeyCode skillBKey;
}
