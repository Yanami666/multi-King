using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 根据 EndlessBook 当前页码，启用/禁用对应的 2D 场景组（Spread）。
/// Enable/disable 2D spread groups based on the current book page.
/// 一组 Spread 对应两页：
/// spreadRoots[0] -> pages 1-2
/// spreadRoots[1] -> pages 3-4
/// spreadRoots[2] -> pages 5-6 ...
/// </summary>
public class Book2DPageManager : MonoBehaviour
{
    [Header("书本引用 / EndlessBook reference")]
    public EndlessBook book;            // 拖 Book 上的 EndlessBook 组件

    [Header("每一组 2D 场景（两页为一组） / One spread = 2 pages")]
    public GameObject[] spreadRoots;    // Spread_01, Spread_02, Spread_03...

    private int _lastPageNumber = -1;

    private void Start()
    {
        if (book != null)
        {
            // 初始按当前页刷新一次
            // Initialize with current page
            UpdateSpreadForPage(book.CurrentPageNumber);
        }
    }

    private void Update()
    {
        if (book == null || spreadRoots == null || spreadRoots.Length == 0)
            return;

        int currentPage = book.CurrentPageNumber;

        if (currentPage != _lastPageNumber)
        {
            _lastPageNumber = currentPage;
            UpdateSpreadForPage(currentPage);
        }
    }

    /// <summary>
    /// 根据当前页码启用对应 spread。
    /// 1-2 -> index 0, 3-4 -> index 1, 5-6 -> index 2 ...
    /// </summary>
    private void UpdateSpreadForPage(int currentPage)
    {
        // 页码从 1 开始，所以先减 1，再除以 2
        // Page numbers start at 1; each spread covers 2 pages.
        int spreadIndex = Mathf.FloorToInt((currentPage - 1) / 2f);

        if (spreadRoots.Length == 0)
            return;

        spreadIndex = Mathf.Clamp(spreadIndex, 0, spreadRoots.Length - 1);

        for (int i = 0; i < spreadRoots.Length; i++)
        {
            if (spreadRoots[i] != null)
            {
                spreadRoots[i].SetActive(i == spreadIndex);
            }
        }

        Debug.Log($"[Book2DPageManager] Current page = {currentPage}, using spread index = {spreadIndex}");
    }
}