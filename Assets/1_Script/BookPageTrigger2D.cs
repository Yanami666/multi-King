using UnityEngine;

/// <summary>
/// 玩家碰到 trigger：
/// 1) FadeOverlay 从当前 alpha 渐变到 1（完全白）
/// 2) 渐变结束的瞬间：切 spread + TurnToPage
/// 3) 再从 1 渐变回 0，露出新页面
///
/// When the player enters the trigger:
/// 1) FadeOverlay fades to 1 (full white)
/// 2) At the moment it reaches 1: switch spread + turn page
/// 3) Then fade back from 1 to 0, revealing the new page.
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

    [Header("Alpha 渐变控制 / Fade overlay")]
    public SpriteAlphaFader fader;   // 拖 FadeOverlay 上的组件

    [Header("渐变时间 / Fade durations")]
    public float fadeToWhiteTime = 0.4f;      // 到白
    public float fadeFromWhiteTime = 0.4f;    // 退白

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

        if (spreadManager == null || fader == null)
        {
            Debug.LogWarning("[BookPageTrigger2D] Missing spreadManager or fader.");
            return;
        }

        _isRunning = true;

        // 第一步：从当前 alpha 渐变到 1（完全白）
        fader.FadeTo(1f, fadeToWhiteTime, () =>
        {
            // ★ 到 1 的这一刻才切 spread
            spreadManager.GoToSpread(targetSpreadIndex, true);

            // 第二步：从 1 渐变回 0，露出新画面
            fader.FadeTo(0f, fadeFromWhiteTime, () =>
            {
                _isRunning = false;
            });
        });
    }
}