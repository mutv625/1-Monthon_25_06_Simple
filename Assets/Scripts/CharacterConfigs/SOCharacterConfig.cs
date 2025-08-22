using UnityEngine;

public class SOCharacterConfig : ScriptableObject
{
    // Character stats
    public float health;
    [Header("移動設定(倍率)")]
    public float speed;
    public float jumpForce;
    public float gravityScale;
}
