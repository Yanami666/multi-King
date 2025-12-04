using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家碰到 trigger 时：
/// 1) 旧画面上渐渐盖上一层白（0 -> 100）
/// 2) 在完全白的时候，直接翻页 + 切换 spread
/// 3) 白色渐渐退去（100 -> 0），只看到新画面从白里出现
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookPageTrigger2D : MonoBehaviour
{
    [Header("Spread 管理器 / Spread manager")]
    public Book2DSpreadManager spreadManager;

    [Header("目标 spread 索引 / Target spread index")]
    public int targetSpreadIndex = 1;

    [Header("玩家 Tag")]
    public string playerTag = "Player";

    [Header("Alpha 渐变控制 / Alpha fade controller")]
    public SpriteAlphaFader fader;   // 拖 FadeOverlay 上的组件

    [Header("渐变时间 / Fade durations")]
    public float fadeToWhiteTime = 0.4f;      // 0 -> 100
    public float fadeFromWhiteTime = 0.4f;    // 100 -> 0

    private bool _isRunning = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isRunning)
            return;

        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        if (spreadManager == null)
            spreadManager = FindObjectOfType<Book2DSpreadManager>();

        if (spreadManager == null)
        {
            Debug.LogWarning("[BookPageTrigger2D] No Book2DSpreadManager found.");
            return;
        }

        if (fader == null)
        {
            Debug.LogWarning("[BookPageTrigger2D] No SpriteAlphaFader assigned.");
            return;
        }

        StartCoroutine(FadeTurnFadeRoutine());
    }

    private IEnumerator FadeTurnFadeRoutine()
    {
        _isRunning = true;

        // 1. 从 0 渐变到 100：旧画面被完全盖住
        yield return fader.FadePercent(0f, 100f, fadeToWhiteTime);

        // 2. 在完全白屏的状态下：翻页 + 切换 spread
        //    ——这一步在白色后面完成，玩家看不到“突然跳图”
        spreadManager.GoToSpread(targetSpreadIndex, true);

        // ★ 不再等 turnTime，直接开始退白，让体验更确定
        // 如果你之后要做很长的 3D 翻页动画，再单独优化。

        // 3. 从 100 渐变到 0：新画面从白色里慢慢显现
        yield return fader.FadePercent(100f, 0f, fadeFromWhiteTime);

        _isRunning = false;
    }
}