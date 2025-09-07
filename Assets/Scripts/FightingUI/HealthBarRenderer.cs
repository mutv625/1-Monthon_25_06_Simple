using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class HealthBarRenderer : MonoBehaviour
{
    [Tooltip("表示する体力を持つプレイヤーの情報")]
    [SerializeField] private PlayerCore playerCore;


    private Image HealthBarImage;
    [SerializeField] private bool isBarRed;

    // 表示中の体力・保留中の体力・遅延解除用Disposable
    private int displayedHealth;
    private int pendingHealth;
    private IDisposable delayDisposable;


    public void Initialize(PlayerCore playerCore)
    {
        this.playerCore = playerCore;
        HealthBarImage = GetComponent<Image>();

        // 初期表示
        displayedHealth = playerCore.currentHealth.Value;
        pendingHealth = displayedHealth;
        UpdateHealthBar(displayedHealth);

        // 体力の変化を監視してUIを更新
        if (isBarRed)
        {
            // currentHealth の変化を監視
            playerCore.currentHealth
                .DistinctUntilChanged()
                .Subscribe(newHealth =>
                {
                    // 回復（増加）は即時反映
                    if (newHealth >= displayedHealth)
                    {
                        // 既存の遅延があれば破棄
                        delayDisposable?.Dispose();
                        delayDisposable = null;

                        displayedHealth = newHealth;
                        pendingHealth = newHealth;
                        UpdateHealthBar(displayedHealth);
                        return;
                    }

                    // 減少（ダメージ）時の処理
                    pendingHealth = newHealth;

                    // ComboState が Trapped の間は保留（反映しない）
                    if (playerCore.comboState.Value == ComboStates.Trapped)
                    {
                        // 保留のみ（反映は Trapped -> 他 に戻ったときに行う）
                        return;
                    }

                    // ComboState が None のときは「1秒間ダメージを受けていなければ」反映する
                    if (playerCore.comboState.Value == ComboStates.None)
                    {
                        // 既存のタイマーをリセットして再起動
                        delayDisposable?.Dispose();
                        delayDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0))
                            .Subscribe(_ =>
                            {
                                displayedHealth = pendingHealth;
                                UpdateHealthBar(displayedHealth);
                                delayDisposable = null;
                            });
                        // AddTo(this) は使わず明示的に Dispose 管理（必要なら AddTo(this) を追加）
                    }
                    else
                    {
                        // その他の状態では即時反映（必要なら変更）
                        displayedHealth = newHealth;
                        UpdateHealthBar(displayedHealth);
                    }
                })
                .AddTo(this);

            // Trapped -> 非Trapped に戻った瞬間に一気に減少を反映
            playerCore.comboState
                .Pairwise()
                .Where(p => p.Previous == ComboStates.Trapped && p.Current != ComboStates.Trapped)
                .Subscribe(_ =>
                {
                    // 保留中のタイマーはキャンセル
                    delayDisposable?.Dispose();
                    delayDisposable = null;

                    displayedHealth = playerCore.currentHealth.Value;
                    pendingHealth = displayedHealth;
                    UpdateHealthBar(displayedHealth);
                })
                .AddTo(this);
        }
        else
        {
            playerCore.currentHealth
                .Subscribe(UpdateHealthBar)
                .AddTo(this);
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        if (playerCore == null || playerCore.maxHealth.Value <= 0)
        {
            Debug.LogWarning("PlayerCore is not set or maxHealth is invalid.");
            return;
        }

        float healthRatio = (float)currentHealth / playerCore.maxHealth.Value;
        HealthBarImage.fillAmount = Mathf.Clamp01(healthRatio);
    }
}
