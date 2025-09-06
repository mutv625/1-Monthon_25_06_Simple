using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ComboDisplayManager : MonoBehaviour
{
    [Tooltip("表示する体力を持つプレイヤーの情報")]
    [SerializeField] private PlayerCore playerCore;

    [Header("子オブジェクト")]
    [SerializeField] private TMPro.TextMeshProUGUI comboCountText;
    [SerializeField] private TMPro.TextMeshProUGUI comboDamageText;

    public void Initialize(PlayerCore playerCore)
    {
        this.playerCore = playerCore;

        // * 0. 初期表示
        UpdateComboDisplay(0, 0);

        // * 1. コンボ数の変化を監視してUIを更新
        playerCore.comboCount
            .DistinctUntilChanged()
            .Subscribe(newComboCount =>
            {
                UpdateComboDisplay(newComboCount, playerCore.comboDamage.Value);
            }).AddTo(this);

        // * 2. 0にリセットされたら非表示
        playerCore.comboCount
            .DistinctUntilChanged()
            .Where(count => count == 0)
            .Subscribe(_ =>
            {
                Display(false);
            }).AddTo(this);
    }

    private void UpdateComboDisplay(int comboCount, int comboDamage)
    {
        comboCountText.text = comboCount.ToString();
        comboDamageText.text = comboDamage.ToString();

        // 必要に応じてアニメーションやエフェクトを追加
    }
    
    private void Display(bool show)
    {
        // 表示アニメーション
    }
}
