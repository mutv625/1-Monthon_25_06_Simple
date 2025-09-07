using UnityEngine;


/// <summary>
/// このコンポーネントをアタッチしたGameObjectの回転を常にリセット（無回転に）する。
/// 親オブジェクトが回転しても、自身の見た目の向きを固定したい場合などに使う。
/// </summary>
public class RotationFixer : MonoBehaviour
{
    void Update()
    {
        // 自身の回転を常に「無回転」の状態にリセットする
        transform.rotation = Quaternion.identity;
    }
}