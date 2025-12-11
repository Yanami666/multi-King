using echo17.EndlessBook;
using UnityEngine;

/// <summary>
/// 玩家 2D 撞到这个 Trigger 时，把 EndlessBook 从封面状态切到 OpenMiddle。
/// 可以选择：
/// - 需要某个 Gate 先解锁
/// - 播放一次开书音效
/// - 在开书动画期间，用 BookPageTurnFlashMaterial 临时切换材质
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookCoverOpenOnTrigger2D : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("EndlessBook 组件 / EndlessBook component")]
    public EndlessBook book;   // 把 BookAnimated 上的 EndlessBook 拖进来

    [Header("打开动画时长 / Open animation time")]
    public float openAnimTime = 0.7f;

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("（可选）需要先解锁的 Gate / Optional gate to require")]
    public Book2DTriggerGate requiredGate;   // 不需要锁就留空

    [Header("（可选）开书音效 / Optional open-book SFX")]
    public AudioSource openSfx;              // 如果要音效，就拖一个 AudioSource

    [Header("（可选）翻页期间临时材质 / Optional flash helper")]
    public BookPageTurnFlashMaterial flashHelper;  // 新脚本，拖 BookAnimated 上的那个

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

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

        hasTriggered = true;

        if (enableDebugLog)
        {
            Debug.Log("[BookCoverOpenOnTrigger2D] Triggered, opening book to OpenMiddle.");
        }

        // 3）有音效就播
        if (openSfx != null)
        {
            openSfx.Play();
        }

        // 4）如果有 FlashHelper，就让它在开书动画期间替换材质
        if (flashHelper != null)
        {
            // 用 openAnimTime 当作闪动时长，这样整段开书动画都在临时材质下进行
            flashHelper.FlashForDuration(openAnimTime);
        }

        // 5）真正开书
        if (book != null)
        {
            // 关键：从封面状态切到 OpenMiddle，带动画时间
            book.SetState(
                EndlessBook.StateEnum.OpenMiddle,
                openAnimTime,
                null
            );
        }
        else
        {
            Debug.LogWarning("[BookCoverOpenOnTrigger2D] Book is not assigned.");
        }
    }
}