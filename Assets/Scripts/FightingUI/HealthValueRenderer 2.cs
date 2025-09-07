using UnityEngine;
using UniRx;

public class HealthValueRenderer : MonoBehaviour
{
    [Tooltip("表示する体力を持つプレイヤーの情報")]
    [SerializeField] private PlayerCore playerCore;

    private TMPro.TextMeshProUGUI HealthValueText;

    public void Initialize(PlayerCore playerCore)
    {
        this.playerCore = playerCore;
        HealthValueText = GetComponent<TMPro.TextMeshProUGUI>();

        // 初期表示
        UpdateHealthValue(playerCore.currentHealth.Value);

        // 体力の変化を監視してUIを更新
        playerCore.currentHealth
            .DistinctUntilChanged()
            .Subscribe(newHealth =>
            {
                UpdateHealthValue(newHealth);
            }).AddTo(this);
    }

    private void UpdateHealthValue(int health)
    {
        HealthValueText.text = health.ToString();
    }
}
