using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 非常简化版：
/// - 不再使用 onCompleted 回调
/// - 不再使用 timeout
/// - 调用 GoToSpread 时，立刻切换对应的 2D 场景（spread）
/// - 如果需要，同时让 EndlessBook 翻到指定页码
///
/// Simple version:
/// - No callbacks, no timeout
/// - When GoToSpread is called, it immediately activates the target 2D spread
/// - Optionally tells EndlessBook to turn to a given page
/// </summary>
public class Book2DSpreadManager : MonoBehaviour
{
    [System.Serializable]
    public class SpreadConfig
    {
        [Header("Inspector 显示用名字 / Label (for Inspector only)")]
        public string label;

        [Header("这一组 2D 场景的根节点 / Root GameObject for this spread")]
        public GameObject spreadRoot;

        [Header("3D 书要翻到的页码（EndlessBook） / Target page in EndlessBook")]
        [Tooltip("If <= 0, we will NOT call TurnToPage when switching to this spread.")]
        public int bookPageNumber = 0; // 封面可以设成 0 -> 不调用 TurnToPage
    }

    [Header("书本引用 / EndlessBook reference")]
    public EndlessBook book;

    [Header("所有 2D 场景配置 / All spreads (0=cover, 1=sp1, 2=sp2...)")]
    public SpreadConfig[] spreads;

    [Header("启动时的 spread 索引 / Start spread index")]
    public int startSpreadIndex = 0;  // 0 = cover

    [Header("启动时书的状态 / Initial book state")]
    public EndlessBook.StateEnum startState = EndlessBook.StateEnum.ClosedFront;
    public float startStateAnimTime = 0f;

    [Header("翻页动画参数 / Page turning settings")]
    public EndlessBook.PageTurnTimeTypeEnum turnTimeType = EndlessBook.PageTurnTimeTypeEnum.TotalTurnTime;
    public float turnTime = 1f;
    public float openTime = 1f;

    private int _currentIndex = -1;

    private void Start()
    {
        if (book == null)
            book = FindObjectOfType<EndlessBook>();

        // 1. 设置书的初始状态（一般是 ClosedFront）
        if (book != null)
        {
            book.SetState(startState, animationTime: startStateAnimTime);
        }

        // 2. 激活起始的 spread（比如 0=封面）
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

        // 1. 如需翻页，通知 EndlessBook
        if (alsoTurnBook && book != null && cfg.bookPageNumber > 0)
        {
            book.TurnToPage(
                cfg.bookPageNumber,
                turnTimeType,
                turnTime,
                openTime: openTime
            );
        }

        // 2. 立刻切换 2D 场景（重点！！！）
        ApplySpread(index);

        Debug.Log($"[Book2DSpreadManager] GoToSpread {index} ({cfg.label}), page={cfg.bookPageNumber}.");
    }

    /// <summary>
    /// 实际激活 / 关闭所有 spreadRoot。
    /// Actually enables/disables spreadRoot GameObjects.
    /// </summary>
    private void ApplySpread(int index)
    {
        if (spreads == null || spreads.Length == 0)
            return;

        index = Mathf.Clamp(index, 0, spreads.Length - 1);

        for (int i = 0; i < spreads.Length; i++)
        {
            if (spreads[i].spreadRoot != null)
            {
                spreads[i].spreadRoot.SetActive(i == index);
            }
        }

        _currentIndex = index;
        Debug.Log($"[Book2DSpreadManager] ApplySpread {index} ({spreads[index].label}).");
    }
}