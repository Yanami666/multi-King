using System.Collections;
using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 玩家 2D 撞到封面 Trigger 时：
/// 1. 白色遮罩从 0% -> 100%
/// 2. 播放开书动画：ClosedFront -> OpenMiddle （EndlessBook）
/// 3. 在白屏状态下切换 2D spread：封面 -> 第一个内页
/// 4. 白色遮罩从 100% -> 0%
///
/// When player hits this 2D trigger on the cover:
/// 1. White overlay fades in (0 -> 100%)
/// 2. Play open-book anim: ClosedFront -> OpenMiddle (EndlessBook)
/// 3. While still white, switch 2D spread: cover -> first inner spread
/// 4. White overlay fades out (100 -> 0%)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookCoverOpenOnTrigger2D : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("EndlessBook 组件 / EndlessBook component")]
    public EndlessBook book;   // 拖 BookAnimated 上的 EndlessBook

    [Header("2D Spread 管理器 / 2D spread manager")]
    public Book2DSpreadManager spreadManager;  // 拖场景里的 Book2DSpreadManager

    [Header("打开动画时长 / Open animation time")]
    [Tooltip("要和 EndlessBook 的开书时间保持一致，例如 0.7")]
    public float openAnimTime = 0.7f;

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("（可选）需要先解锁的 Gate / Optional gate to require")]
    public Book2DTriggerGate requiredGate;   // 不需要锁就留空

    [Header("（可选）开书音效 / Optional open-book SFX")]
    public AudioSource openSfx;              // 如果要音效，就拖一个 AudioSource

    [Header("白色遮罩 Fader / White overlay fader")]
    public SpriteAlphaFader fadeOverlay;     // 拖 FadeOverlay 上的 SpriteAlphaFader

    [Header("遮罩时间设置 / Fade timings")]
    public float fadeInTime = 0.2f;       // 渐白时间
    public float whiteHoldTime = 0.5f;       // 全白保持时间（至少 ≧ openAnimTime 比较安全）
    public float fadeOutTime = 0.2f;       // 渐隐时间

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private bool isRunning = false;

    private void Reset()
    {
        // 自动把自己的 Collider2D 设成 Trigger
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只认 Player
        if (!other.CompareTag(playerTag))
            return;

        // 1）先看 Gate 锁有没有开（如果有设置的话）
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
            {
                Debug.Log("[BookCoverOpenOnTrigger2D] Gate still LOCKED, ignore trigger. Gate = "
                          + requiredGate.name);
            }
            return;
        }

        // 2）只触发一次的话，第二次以后直接忽略
        if (triggerOnlyOnce && hasTriggered)
        {
            if (enableDebugLog)
            {
                Debug.Log("[BookCoverOpenOnTrigger2D] Already triggered once, skip.");
            }
            return;
        }

        if (isRunning)
            return;

        hasTriggered = true;
        StartCoroutine(CoOpenCoverWithFade());
    }

    private IEnumerator CoOpenCoverWithFade()
    {
        isRunning = true;

        if (enableDebugLog)
            Debug.Log("[BookCoverOpenOnTrigger2D] Start open cover with fade.");

        // 0）播放音效
        if (openSfx != null)
        {
            openSfx.Play();
        }

        // 1）渐变白：0 -> 100
        if (fadeOverlay != null)
        {
            yield return fadeOverlay.FadePercent(0f, 100f, fadeInTime);
        }

        // 2）开始开书动画：ClosedFront -> OpenMiddle
        if (book != null)
        {
            // 这里用 openAnimTime 和 EndlessBook 同步
            book.SetState(
                EndlessBook.StateEnum.OpenMiddle,
                openAnimTime,
                null
            );
        }
        else if (enableDebugLog)
        {
            Debug.LogWarning("[BookCoverOpenOnTrigger2D] Book is not assigned.");
        }

        // 3）等待开书动画完成（假设 openAnimTime 是准确的）
        float waitTime = Mathf.Max(openAnimTime, 0f);
        if (whiteHoldTime > waitTime)
        {
            // 先等开书动画时间
            if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);

            // 再多等一会儿白屏
            yield return new WaitForSeconds(whiteHoldTime - waitTime);
        }
        else
        {
            // 只等开书动画时间（或者 whiteHoldTime 更短就只等 whiteHoldTime）
            if (whiteHoldTime > 0f)
                yield return new WaitForSeconds(whiteHoldTime);
            else if (waitTime > 0f)
                yield return new WaitForSeconds(waitTime);
        }

        // 4）在白屏状态下，把 2D spread 从封面切到第一个内页
        if (spreadManager != null)
        {
            spreadManager.SwitchFromCoverToFirstInnerSpread();
        }
        else if (enableDebugLog)
        {
            Debug.LogWarning("[BookCoverOpenOnTrigger2D] SpreadManager is not assigned.");
        }

        // 5）白色遮罩渐隐：100 -> 0
        if (fadeOverlay != null)
        {
            yield return fadeOverlay.FadePercent(100f, 0f, fadeOutTime);
        }

        if (enableDebugLog)
            Debug.Log("[BookCoverOpenOnTrigger2D] Done open cover with fade.");

        isRunning = false;
    }
}