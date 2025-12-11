using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家碰到 / 点击这个 2D trigger：
/// 1. 白色遮罩从 0% -> 100%（fade in）
/// 2. 调 Book2DSpreadManager 去下一页或上一页（内部会播 EndlessBook 翻页动画）
/// 3. 白色保持一小会（whiteHoldTime）
/// 4. 白色遮罩从 100% -> 0%（fade out）
///
/// Player enters / clicks this 2D trigger:
/// 1. White overlay fades in (0 -> 100%)
/// 2. Book2DSpreadManager turns page (EndlessBook anim)
/// 3. Hold white for a moment
/// 4. White overlay fades out (100 -> 0%)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookPageTrigger2D : MonoBehaviour
{
    [Header("触发方式 / Trigger mode")]
    public bool triggerOnPlayerEnter = true;   // 玩家碰上去
    public bool triggerOnMouseClick = false;   // 或者用鼠标点这个物体

    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = false;
    private bool _hasTriggered = false;

    [Header("翻页方向 / Page direction")]
    public bool goNext = true;                 // true = 下一页，false = 上一页

    [Header("书本 spread 管理器 / Spread manager")]
    public Book2DSpreadManager spreadManager;  // 拖 Book2DSpreadManager

    [Header("白色遮罩 Fader / White overlay fader")]
    public SpriteAlphaFader fadeOverlay;       // 拖 FadeOverlay 上的 SpriteAlphaFader

    [Header("遮罩时间设置 / Fade timings")]
    public float fadeInTime = 0.2f;         // 渐白时间
    public float whiteHoldTime = 0.5f;         // 全白保持时间（建议 >= 翻页动画时间）
    public float fadeOutTime = 0.2f;         // 渐隐时间

    private bool _isRunning = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnPlayerEnter)
            return;

        if (!other.CompareTag(playerTag))
            return;

        TryStartPageTurn();
    }

    private void OnMouseDown()
    {
        if (!triggerOnMouseClick)
            return;

        TryStartPageTurn();
    }

    private void TryStartPageTurn()
    {
        if (_isRunning)
            return;

        if (triggerOnlyOnce && _hasTriggered)
            return;

        _hasTriggered = true;
        StartCoroutine(CoPageTurnWithFade());
    }

    private IEnumerator CoPageTurnWithFade()
    {
        _isRunning = true;

        // 1. 渐变白
        if (fadeOverlay != null)
        {
            yield return fadeOverlay.FadePercent(0f, 100f, fadeInTime);
        }

        // 2. 调用 SpreadManager 翻页（内部会播 EndlessBook 动画）
        if (spreadManager != null)
        {
            if (goNext)
                spreadManager.GoToNextSpread();
            else
                spreadManager.GoToPreviousSpread();
        }

        // 3. 全白保持一段时间（确保动画在白屏之下完成）
        if (whiteHoldTime > 0f)
        {
            yield return new WaitForSeconds(whiteHoldTime);
        }

        // 4. 渐隐白
        if (fadeOverlay != null)
        {
            yield return fadeOverlay.FadePercent(100f, 0f, fadeOutTime);
        }

        _isRunning = false;
    }
}