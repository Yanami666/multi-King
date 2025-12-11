using UnityEngine;

/// <summary>
/// 非常简单的翻页 Trigger：
/// - 可以用 2D Trigger（玩家碰到）
/// - 或者 OnMouseDown（鼠标点）
/// - 调用 Book2DSpreadManager.GoToSpread(targetSpreadIndex)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BookPageTrigger2D : MonoBehaviour
{
    [Header("哪个 SpreadManager / Spread manager")]
    public Book2DSpreadManager spreadManager;

    [Header("目标页索引 / Target spread index")]
    public int targetSpreadIndex = 0;

    [Header("是否用 2D Trigger（玩家碰到）")]
    public bool usePlayerTrigger = true;

    [Header("是否用鼠标点击这个 Collider")]
    public bool useMouseClick = false;

    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = true;

    private bool _hasTriggered = false;

    private void Reset()
    {
        // 自动把 Collider2D 设成 Trigger，方便用 OnTriggerEnter2D
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!usePlayerTrigger)
            return;

        if (!other.CompareTag(playerTag))
            return;

        TryTurnPage("OnTriggerEnter2D");
    }

    private void OnMouseDown()
    {
        if (!useMouseClick)
            return;

        TryTurnPage("OnMouseDown");
    }

    private void TryTurnPage(string fromWhere)
    {
        if (triggerOnlyOnce && _hasTriggered)
            return;

        if (spreadManager == null)
        {
            Debug.LogWarning("[BookPageTrigger2D] spreadManager 没有指定。From = " + fromWhere);
            return;
        }

        spreadManager.GoToSpread(targetSpreadIndex);
        _hasTriggered = true;
    }
}