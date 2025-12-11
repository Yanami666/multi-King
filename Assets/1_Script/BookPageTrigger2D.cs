using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家 2D 撞到 Trigger 时翻页。
/// 可以：
/// - 走固定 targetSpreadIndex
/// - 或者 goToNext / goToPrev
/// - 可选：需要 Gate 解锁
/// - 可选：第一页翻书时白屏闪一下再淡出
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookPageTrigger2D : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("翻页管理器 / Spread manager")]
    public Book2DSpreadManager spreadManager;

    [Header("目标页设置 / Target spread")]
    public int targetSpreadIndex = 0;
    public bool goToNext = false;
    public bool goToPrev = false;

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("（可选）需要 Gate 解锁 / Optional gate")]
    public Book2DTriggerGate requiredGate;

    [Header("（可选）第一页翻书白屏闪一下 / White flash on first cover turn")]
    public bool useCoverWhiteFlash = false;        // 只在封面那个 trigger 上勾选
    public SpriteAlphaFader fadeOverlay;           // 场景里那张全屏白图上挂 SpriteAlphaFader
    public float whiteHoldTime = 0.5f;             // 白板维持时间
    public float whiteFadeTime = 0.5f;             // 淡出时间

    [Header("（可选）翻页音效 / Page turn SFX")]
    public AudioSource pageTurnSfx;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只认 Player
        if (!other.CompareTag(playerTag))
            return;

        // Gate 还没解锁就不翻
        if (requiredGate != null && !requiredGate.IsUnlocked)
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (spreadManager == null)
        {
            Debug.LogWarning("[BookPageTrigger2D] spreadManager 没拖。");
            return;
        }

        // 计算要去的 spread
        int target = targetSpreadIndex;

        if (goToNext)
            target = spreadManager.CurrentSpreadIndex + 1;
        else if (goToPrev)
            target = spreadManager.CurrentSpreadIndex - 1;

        // 播放翻页音效（有的话）
        if (pageTurnSfx != null)
            pageTurnSfx.Play();

        // 如果勾了“封面白屏”，就走白屏流程
        if (useCoverWhiteFlash && fadeOverlay != null)
        {
            StartCoroutine(WhiteFlashAndTurn(target));
        }
        else
        {
            // 普通翻页，什么遮罩都不干预
            spreadManager.GoToSpread(target);
        }
    }

    /// <summary>
    /// 封面翻页：先让整本书被一块白板盖住，再等 0.5s，之后淡出。
    /// 页面翻页动画在白板下面正常跑，这样观众看不到那一帧 bug。
    /// </summary>
    private IEnumerator WhiteFlashAndTurn(int spreadIndex)
    {
        // 1. 立刻全白
        fadeOverlay.SetAlpha(1f);

        // 2. 立刻开始翻页动画（此时玩家只看到一张白板）
        spreadManager.GoToSpread(spreadIndex);

        // 3. 白板保持一段时间
        yield return new WaitForSeconds(whiteHoldTime);

        // 4. 用百分比 100 -> 0 淡出
        yield return fadeOverlay.FadePercent(100f, 0f, whiteFadeTime);
    }
}