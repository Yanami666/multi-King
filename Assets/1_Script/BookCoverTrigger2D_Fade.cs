using System.Collections;
using UnityEngine;
using echo17.EndlessBook;   // EndlessBook 命名空间

/// <summary>
/// 封面专用：
/// 1. 玩家碰到 2D Trigger
/// 2. 白幕从 0 -> 100（盖住画面）
/// 3. 在全白时，把书从 ClosedFront 播到 OpenMiddle（封面打开到中间）
/// 4. 同时 2D spread 切到第一页
/// 5. 等开书动画时间结束后，再让白幕从 100 -> 0
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookCoverTrigger2D_Fade : MonoBehaviour
{
    [Header("3D 书本引用 / EndlessBook reference")]
    public EndlessBook book;                 // 拖有 EndlessBook 组件的那个 Book 对象

    [Header("2D Spread 管理器 / 2D spread manager")]
    public Book2DSpreadManager spreadManager; // 拖你现用的 Book2DSpreadManager

    [Header("封面要跳到的目标 Spread 索引 / Target spread index")]
    public int targetSpreadIndex = 1;        // 0 = 封面, 1 = 第一页 (按你项目来改)

    [Header("白幕控制 / White overlay fader")]
    public SpriteAlphaFader fader;           // 拖 FadeOverlay 上的 SpriteAlphaFader

    [Header("渐变时间 / Fade durations")]
    public float fadeToWhiteTime = 0.2f;     // 0 -> 100 的时间（尽量短一点）
    public float fadeFromWhiteTime = 0.4f;   // 100 -> 0 的时间（慢慢出现）

    [Header("开书动画时长 / Open animation duration (seconds)")]
    [Tooltip("要和 EndlessBook 里封面打开那段动画时间差不多，例如 1.0 或 1.2")]
    public float openAnimationTime = 1.0f;

    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("触发期间是否锁玩家移动 / Lock 2D player movement while fading")]
    public bool lockPlayerMove = true;
    public MonoBehaviour playerMoveScript;   // 你的 2D 玩家移动脚本（不想锁可以留空）

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private bool _hasTriggered = false;
    private bool _isRunning = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isRunning)
            return;

        if (!other.CompareTag(playerTag))
            return;

        if (triggerOnlyOnce && _hasTriggered)
            return;

        StartCoroutine(CoverOpenRoutine());
    }

    private IEnumerator CoverOpenRoutine()
    {
        _isRunning = true;
        _hasTriggered = true;

        if (enableDebugLog)
            Debug.Log("[BookCoverTrigger2D_Fade] Start cover open with white overlay.");

        // 1. 锁玩家移动（可选）
        if (lockPlayerMove && playerMoveScript != null)
            playerMoveScript.enabled = false;

        // 2. 白幕 0 -> 100，先把画面盖住
        if (fader != null)
        {
            yield return fader.FadePercent(0f, 100f, fadeToWhiteTime);
        }

        // 3. 在全白的状态下，开始播放封面打开动画 + 切换 2D spread
        if (book != null)
        {
            // 从当前状态（ClosedFront）切到 OpenMiddle
            // 注意：openAnimationTime 就是动画时长
            book.SetState(EndlessBook.StateEnum.OpenMiddle,
                          openAnimationTime,
                          null);

            if (enableDebugLog)
                Debug.Log("[BookCoverTrigger2D_Fade] Book.SetState -> OpenMiddle, time = " + openAnimationTime);
        }

        if (spreadManager != null)
        {
            // 2D 直接瞬移到第一页（不用再渐变）
            spreadManager.GoToSpread(targetSpreadIndex, true);

            if (enableDebugLog)
                Debug.Log("[BookCoverTrigger2D_Fade] GoToSpread index = " + targetSpreadIndex + " (instant).");
        }

        // 4. 在白幕 100% 的情况下，等开书动画播完
        if (openAnimationTime > 0f)
        {
            yield return new WaitForSeconds(openAnimationTime);
        }

        // 5. 动画已经结束，此时书已经稳定在 page1，再让白幕从 100 -> 0
        if (fader != null)
        {
            yield return fader.FadePercent(100f, 0f, fadeFromWhiteTime);
        }

        // 6. 解锁玩家移动（可选）
        if (lockPlayerMove && playerMoveScript != null)
            playerMoveScript.enabled = true;

        // 7. 用完就关掉这个 Trigger（可选）
        if (triggerOnlyOnce)
            gameObject.SetActive(false);

        _isRunning = false;

        if (enableDebugLog)
            Debug.Log("[BookCoverTrigger2D_Fade] Cover open finished.");
    }
}