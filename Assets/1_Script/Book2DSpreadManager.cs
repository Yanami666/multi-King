using UnityEngine;

/// <summary>
/// 管 2D 页面显示哪个 spread：
/// - spreads 数组里放每一页的根节点（sp1, sp2, sp3...）
/// - GoToSpread(index) 打开那一页，其他都关掉
/// </summary>
public class Book2DSpreadManager : MonoBehaviour
{
    [Header("所有 2D 页面根对象 / All spread roots")]
    public GameObject[] spreads;

    [Header("初始页索引 / Start spread index")]
    public int startSpreadIndex = 0;

    public int CurrentSpreadIndex { get; private set; } = 0;

    private void Start()
    {
        // 开局直接跳到 startSpreadIndex
        ShowSpread(startSpreadIndex);
    }

    /// <summary>
    /// 跳到某一页（索引从 0 开始）
    /// </summary>
    public void GoToSpread(int index)
    {
        if (spreads == null || spreads.Length == 0)
        {
            Debug.LogWarning("[Book2DSpreadManager] spreads 数组是空的。");
            return;
        }

        if (index < 0 || index >= spreads.Length)
        {
            Debug.LogWarning("[Book2DSpreadManager] GoToSpread index 越界: " + index);
            return;
        }

        ShowSpread(index);
    }

    /// <summary>
    /// 下一页 / 上一页（如果你以后想用）
    /// </summary>
    public void GoToNextSpread()
    {
        GoToSpread(CurrentSpreadIndex + 1);
    }

    public void GoToPreviousSpread()
    {
        GoToSpread(CurrentSpreadIndex - 1);
    }

    private void ShowSpread(int index)
    {
        for (int i = 0; i < spreads.Length; i++)
        {
            if (spreads[i] != null)
                spreads[i].SetActive(i == index);
        }

        CurrentSpreadIndex = index;
    }
}