using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEditor.Animations;

public class ComboDisplayManager : MonoBehaviour
{
    [Tooltip("表示する体力を持つプレイヤーの情報")]
    [SerializeField] private PlayerCore playerCore;

    [Header("子オブジェクト")]
    [SerializeField] private TMPro.TextMeshProUGUI comboCountText;
    [SerializeField] private TMPro.TextMeshProUGUI comboDamageText;

    // ゲージ表示用
    private Image comboGaugeImage;
    private CanvasGroup canvasGroup;
    private Animator animator;

    public void Initialize(PlayerCore playerCore)
    {
        this.playerCore = playerCore;
        comboGaugeImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();


        // # コンボの数値表示
        // * 0. 初期表示
        UpdateComboDisplay(0, 0);

        // * 1. コンボ数の変化を監視してUIを更新
        playerCore.comboCount
            .DistinctUntilChanged()
            .Subscribe(newComboCount =>
            {
                Display(newComboCount > 0);
                if (newComboCount > 0) UpdateComboDisplay(newComboCount, playerCore.comboDamage.Value);
            }).AddTo(this);

        // * 2. 0にリセットされたら非表示
        playerCore.comboState
            .Where(state => state == ComboStates.None || state == ComboStates.Ending)
            .Subscribe(_ => Display(false))
            .AddTo(this);

        // # コンボゲージ表示
        playerCore.comboGaugeValue
            .DistinctUntilChanged()
            .Subscribe(value => UpdateComboGauge(value))
            .AddTo(this);
    }

    private void UpdateComboDisplay(int comboCount, int comboDamage)
    {
        comboCountText.text = comboCount.ToString();
        comboDamageText.text = comboDamage.ToString();

        // 必要に応じてアニメーションやエフェクトを追加
    }

    private void UpdateComboGauge(float comboGaugeValue)
    {
        if (playerCore == null) return;
        float gaugeRatio = Mathf.Clamp01(comboGaugeValue / 100);

        comboGaugeImage.fillAmount = gaugeRatio;
    }

    private void Display(bool show)
    {
        if (show)
        {
            animator.SetTrigger("trigPopUp");
            animator.SetBool("isShown", true);
        }
        else
        {
            animator.SetBool("isShown", false);
        }

    }
}
