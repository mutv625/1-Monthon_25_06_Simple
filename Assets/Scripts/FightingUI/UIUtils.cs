using UnityEngine;
using UnityEngine.UI;

public class UIUtils
{
    public static void SetVisibleRecursively(GameObject root, bool visible)
    {
        if (root == null) return;

        // 全ての Renderer (MeshRenderer, SpriteRenderer, SkinnedMeshRenderer 等)
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) r.enabled = visible;

        // UI の Graphic (Image, Text など)
        var graphics = root.GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics) g.enabled = visible;

        // CanvasGroup が使われている場合は alpha で見た目を制御し、入力も制御
        var canvasGroups = root.GetComponentsInChildren<CanvasGroup>(true);
        foreach (var cg in canvasGroups)
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }

        // パーティクルのレンダラー（見た目のみ）
        var psRenderers = root.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (var pr in psRenderers) pr.enabled = visible;
    }
}

