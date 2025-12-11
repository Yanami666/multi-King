using System.Collections;
using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 管整个 2D 书本：
/// - 管理 2D spreads 的显隐（封面、第一页、第二页……）
/// - 调 EndlessBook 做真正的翻书动画
/// - 不负责白色遮罩、不负责输入，只提供 API 给别的脚本调用。
///
/// Manages 2D spreads + EndlessBook page animations.
/// No input, no white overlay here.
/// </summary>
public class Book2DSpreadManager : MonoBehaviour
{
    [Header("EndlessBook 组件 / EndlessBook component")]
    public EndlessBook book;          // 拖 BookAnimated 上的 EndlessBook

    [Header("所有 2D spreads（顺序要和页码对应）")]
    [Tooltip("Element 0 建议是封面 F_cover；Element 1 开始是内页 1-2, 3-4...")]
    public GameObject[] spreads;

    [Header("初始 spread Index（0-based）")]
    public int startSpreadIndex = 0;

    [Header("游戏开始时是否从封面合上开始？")]
    public bool startWithBookClosedFront = true;

    [Header("开书 & 翻页动画时间 / Open & page turn times")]
    public float openTime = 0.7f;     // 封面 <-> 中间
    public float pageTurnTime = 0.5f; // 中间页之间翻页

    /// <summary>当前是哪个 spread（0-based）</summary>
    public int CurrentSpreadIndex => _currentSpreadIndex;

    private int _currentSpreadIndex;
    private bool _isTurning = false;

    private void Awake()
    {
        // 安全检查
        if (book == null)
        {
            book = FindObjectOfType<EndlessBook>();
        }
    }

    private void Start()
    {
        // 先全部关掉
        if (spreads != null)
        {
            for (int i = 0; i < spreads.Length; i++)
            {
                if (spreads[i] != null)
                    spreads[i].SetActive(false);
            }
        }

        // 确定初始 index
        _currentSpreadIndex = Mathf.Clamp(startSpreadIndex, 0,
            spreads != null && spreads.Length > 0 ? spreads.Length - 1 : 0);

        // 设置 EndlessBook 初始状态
        if (book != null)
        {
            if (startWithBookClosedFront)
            {
                // 从封面合上状态开始（ClosedFront）
                book.SetState(EndlessBook.StateEnum.ClosedFront, 0f, null);
            }
            else
            {
                // 直接从中间打开状态开始
                book.SetState(EndlessBook.StateEnum.OpenMiddle, 0f, null);
            }
        }

        // 显示初始 spread（一般是 F_cover）
        ShowSpread(_currentSpreadIndex);
    }

    /// <summary>
    /// 外部调用：下一张 spread。
    /// Called by triggers to go to next spread.
    /// </summary>
    public void GoToNextSpread()
    {
        if (_isTurning || spreads == null || spreads.Length == 0)
            return;

        int target = _currentSpreadIndex + 1;
        if (target >= spreads.Length)
            return;

        StartCoroutine(CoGoToSpread(target));
    }

    /// <summary>
    /// 外部调用：上一张 spread。
    /// Go to previous spread.
    /// </summary>
    public void GoToPreviousSpread()
    {
        if (_isTurning || spreads == null || spreads.Length == 0)
            return;

        int target = _currentSpreadIndex - 1;
        if (target < 0)
            return;

        StartCoroutine(CoGoToSpread(target));
    }

    /// <summary>
    /// 封面打开后，强制切到第一个内页（Element 1）。
    /// 供 BookCoverOpenOnTrigger2D 在开书动画结束后调用。
    ///
    /// Force switch to first inner spread (index 1) after cover opens.
    /// </summary>
    public void SwitchFromCoverToFirstInnerSpread()
    {
        if (spreads == null || spreads.Length < 2)
            return;

        ShowSpread(1);
        _currentSpreadIndex = 1;
    }

    /// <summary>
    /// 真正翻页逻辑：处理封面 <-> 中间、以及中间页之间翻页。
    /// 实际动画都在这里调 EndlessBook。
    /// </summary>
    private IEnumerator CoGoToSpread(int targetIndex)
    {
        _isTurning = true;

        int oldIndex = _currentSpreadIndex;

        // 1）封面 -> 第一页：ClosedFront -> OpenMiddle
        if (oldIndex == 0 && targetIndex > 0)
        {
            if (book != null)
            {
                book.SetState(EndlessBook.StateEnum.OpenMiddle, openTime, null);
            }
            yield return new WaitForSeconds(openTime);
        }
        // 2）第一页 -> 封面：OpenMiddle -> ClosedFront（看你以后要不要）
        else if (oldIndex > 0 && targetIndex == 0)
        {
            if (book != null)
            {
                book.SetState(EndlessBook.StateEnum.ClosedFront, openTime, null);
            }
            yield return new WaitForSeconds(openTime);
        }
        // 3）中间页之间翻页（有动画）
        else if (oldIndex > 0 && targetIndex > 0)
        {
            if (book != null)
            {
                if (targetIndex > oldIndex)
                {
                    // 向后翻一张 spread（2 页）
                    book.TurnForward(pageTurnTime);
                }
                else
                {
                    // 向前翻一张 spread
                    book.TurnBackward(pageTurnTime);
                }
            }
            yield return new WaitForSeconds(pageTurnTime);
        }

        // 4）动画结束后，切换 2D spread 显示
        ShowSpread(targetIndex);
        _currentSpreadIndex = targetIndex;

        _isTurning = false;
    }

    /// <summary>
    /// 只负责显隐 2D spreads。
    /// </summary>
    private void ShowSpread(int index)
    {
        if (spreads == null || spreads.Length == 0)
            return;

        for (int i = 0; i < spreads.Length; i++)
        {
            if (spreads[i] != null)
                spreads[i].SetActive(i == index);
        }
    }
}