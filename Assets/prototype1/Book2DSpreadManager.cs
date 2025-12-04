using System.Collections;
using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 管理书上所有 2D 场景组（spc, sp1, sp2 ...）
/// 切换时：
///  - 调用 EndlessBook.TurnToPage 播放翻页动画；
///  - 优先用 onCompleted 回调，在动画结束那一刻切 2D；
///  - 如果回调没触发，用一个很短的超时兜底。
///
/// Manages all 2D spreads (spc, sp1, sp2 ...).
/// When switching:
///  - Calls EndlessBook.TurnToPage to animate page turn;
///  - Uses onCompleted to switch 2D exactly when the animation ends;
///  - If callback doesn't fire, uses a short timeout as a fallback.
/// </summary>
public class Book2DSpreadManager : MonoBehaviour
{
    [System.Serializable]
    public class SpreadConfig
    {
        [Header("名字（只是方便在 Inspector 里看） / Display label")]
        public string label;

        [Header("这一页 2D 场景的根节点 / Root GameObject for this spread")]
        public GameObject spreadRoot;

        [Header("3D 书要翻到的页码（EndlessBook 里的 Page 编号）")]
        [Tooltip("If <= 0, no TurnToPage is called when switching to this spread.")]
        public int bookPageNumber = 0; // <=0 表示切过去时不翻 3D 页
    }

    [Header("书本引用 / EndlessBook reference")]
    public EndlessBook book;

    [Header("所有 2D 场景配置 / All 2D spreads (0=spc, 1=sp1, 2=sp2...)")]
    public SpreadConfig[] spreads;

    [Header("启动时的场景索引（比如 0=封面） / Start spread index")]
    public int startSpreadIndex = 0;

    [Header("启动时书的状态 / Initial book state")]
    public EndlessBook.StateEnum startState = EndlessBook.StateEnum.ClosedFront;
    public float startStateAnimTime = 0f;

    [Header("翻页动画参数 / Page turning settings")]
    public EndlessBook.PageTurnTimeTypeEnum turnTimeType = EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime;
    public float turnTime = 1f;
    public float openTime = 1f;

    [Header("兜底等待的额外时间 / Extra max wait if callback fails")]
    [Tooltip("If onCompleted is never called, we force switch after (turnTime + extraMaxWait).")]
    public float extraMaxWait = 0.1f;

    private int _currentIndex = -1;

    // 正在翻页？
    private bool _isTurning = false;
    // 等动画结束后要切到的目标 spread
    private int _pendingIndex = -1;
    private Coroutine _timeoutRoutine = null;

    private void Start()
    {
        if (book == null)
            book = FindObjectOfType<EndlessBook>();

        if (book != null)
        {
            // 启动时设置书的状态（一般是封面合上）
            book.SetState(startState, animationTime: startStateAnimTime);
        }

        // 启动时只激活封面 2D，不翻页
        ApplySpread(startSpreadIndex);
        Debug.Log($"[Book2DSpreadManager] Start at spread {startSpreadIndex}.");
    }

    /// <summary>
    /// 对外接口：切换到某个 spread。
    /// Switch to a given spread index.
    /// </summary>
    public void GoToSpread(int index, bool alsoTurnBook = true)
    {
        if (spreads == null || spreads.Length == 0)
        {
            Debug.LogWarning("[Book2DSpreadManager] No spreads configured.");
            return;
        }

        index = Mathf.Clamp(index, 0, spreads.Length - 1);
        SpreadConfig cfg = spreads[index];

        // 不需要翻页：直接切 2D
        if (!alsoTurnBook || book == null || cfg.bookPageNumber <= 0)
        {
            ApplySpread(index);
            Debug.Log($"[Book2DSpreadManager] GoToSpread {index} ({cfg.label}) without page turn.");
            return;
        }

        // 正在翻页时，忽略新的请求，避免卡 bug
        if (_isTurning)
        {
            Debug.Log($"[Book2DSpreadManager] Already turning page, ignore request to {index} ({cfg.label}).");
            return;
        }

        _isTurning = true;
        _pendingIndex = index;

        // 开始翻页动画，注册 onCompleted 回调
        book.TurnToPage(
            cfg.bookPageNumber,
            turnTimeType,
            turnTime,
            openTime: openTime,
            onCompleted: OnBookTurnToPageCompleted,
            onPageTurnStart: null,
            onPageTurnEnd: null
        );

        Debug.Log($"[Book2DSpreadManager] Start turning to page {cfg.bookPageNumber} for spread {index} ({cfg.label}).");

        // 开一个很短的兜底计时，如果回调没有被调到，就强制切换
        if (_timeoutRoutine != null)
            StopCoroutine(_timeoutRoutine);
        _timeoutRoutine = StartCoroutine(PageTurnTimeoutRoutine());
    }

    /// <summary>
    /// 真正执行 2D 场景激活/隐藏。
    /// Actually enable/disable all spreadRoot GameObjects.
    /// </summary>
    private void ApplySpread(int index)
    {
        if (spreads == null || spreads.Length == 0)
            return;

        index = Mathf.Clamp(index, 0, spreads.Length - 1);

        for (int i = 0; i < spreads.Length; i++)
        {
            if (spreads[i].spreadRoot != null)
                spreads[i].spreadRoot.SetActive(i == index);
        }

        _currentIndex = index;
        Debug.Log($"[Book2DSpreadManager] ApplySpread {index} ({spreads[index].label}).");
    }

    /// <summary>
    /// EndlessBook 翻页动画完成时的回调。
    /// Called when EndlessBook finishes TurnToPage.
    /// </summary>
    private void OnBookTurnToPageCompleted(EndlessBook.StateEnum fromState,
                                           EndlessBook.StateEnum toState,
                                           int currentPageNumber)
    {
        // 如果已经不在翻页状态，就什么都不做（可能是超时兜底先执行了）
        if (!_isTurning)
            return;

        _isTurning = false;

        if (_timeoutRoutine != null)
        {
            StopCoroutine(_timeoutRoutine);
            _timeoutRoutine = null;
        }

        if (_pendingIndex >= 0)
        {
            int target = _pendingIndex;
            _pendingIndex = -1;
            ApplySpread(target);
            Debug.Log($"[Book2DSpreadManager] Page turn completed (currentPage={currentPageNumber}), switched to spread {target}.");
        }
    }

    /// <summary>
    /// 如果 onCompleted 没有触发，在 turnTime + extraMaxWait 之后强制切换。
    /// If onCompleted never fires, force switch after (turnTime + extraMaxWait).
    /// </summary>
    private IEnumerator PageTurnTimeoutRoutine()
    {
        float waitTime = Mathf.Max(0.05f, turnTime + extraMaxWait);
        yield return new WaitForSeconds(waitTime);

        if (_isTurning && _pendingIndex >= 0)
        {
            int target = _pendingIndex;
            _isTurning = false;
            _pendingIndex = -1;

            ApplySpread(target);
            Debug.Log($"[Book2DSpreadManager] Timeout reached, forced switch to spread {target}.");
        }

        _timeoutRoutine = null;
    }
}