using UnityEngine;

public class Book2DClickManager : MonoBehaviour
{
    [Header("用于检测点击的相机（一般拖 BookCam） / Camera used for 2D click")]
    public Camera book2DCamera;

    [Header("点击检测的 LayerMask（建议只勾 Book2D） / Layer mask for click test")]
    public LayerMask clickableLayerMask = ~0; // default: everything

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private void Awake()
    {
        if (book2DCamera == null)
        {
            // 如果没拖，就尝试用 MainCamera
            // Try using MainCamera if none is assigned
            book2DCamera = Camera.main;

            if (enableDebugLog)
            {
                Debug.Log("[Book2DClickManager] book2DCamera not assigned, using Camera.main");
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }
    }

    /// <summary>
    /// 处理一次鼠标左键点击
    /// Handle a single left-mouse click
    /// </summary>
    private void HandleClick(Vector3 mousePos)
    {
        if (book2DCamera == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("[Book2DClickManager] No camera assigned.");
            }
            return;
        }

        // 把屏幕坐标转换为 2D 世界坐标（只要 x,y）
        // Convert screen pos to 2D world pos
        Vector3 world = book2DCamera.ScreenToWorldPoint(mousePos);
        Vector2 point = new Vector2(world.x, world.y);

        // 找到这个点下的所有 2D Collider
        // Get all 2D colliders under this point
        Collider2D[] hits = Physics2D.OverlapPointAll(point, clickableLayerMask);

        if (hits == null || hits.Length == 0)
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DClickManager] No 2D hit at " + point);
            }
            return;
        }

        // 从所有 hit 中，选出 SpriteRenderer.sortingOrder 最大的那个
        // Pick the one with the highest sortingOrder
        Collider2D chosen = hits[0];
        int bestOrder = GetSortingOrder(hits[0]);

        for (int i = 1; i < hits.Length; i++)
        {
            int order = GetSortingOrder(hits[i]);
            if (order > bestOrder)
            {
                bestOrder = order;
                chosen = hits[i];
            }
        }

        GameObject target = chosen.gameObject;

        if (enableDebugLog)
        {
            Debug.Log("[Book2DClickManager] Clicked object: " + target.name + " (sortingOrder=" + bestOrder + ")");
        }

        // 依次尝试四种组件
        // Try four handlers in order

        // 1. 渐隐消失 / fade out
        Book2DItemClickFadeOut fade = target.GetComponent<Book2DItemClickFadeOut>();
        if (fade != null)
        {
            if (enableDebugLog) Debug.Log("[Book2DClickManager] -> Book2DItemClickFadeOut on " + target.name);
            fade.StartFadeFromExternal();
        }

        // 2. 移动一次 / move once
        Book2DItemClickMoveOnce move = target.GetComponent<Book2DItemClickMoveOnce>();
        if (move != null)
        {
            if (enableDebugLog) Debug.Log("[Book2DClickManager] -> Book2DItemClickMoveOnce on " + target.name);
            move.OnExternalClick();
        }

        // 3. 播放动画 / play animation
        Book2DClickPlayAnimSimple anim = target.GetComponent<Book2DClickPlayAnimSimple>();
        if (anim != null)
        {
            if (enableDebugLog) Debug.Log("[Book2DClickManager] -> Book2DClickPlayAnimSimple on " + target.name);
            anim.PlayFromExternal();
        }

        // 4. 触发其它对象出现/消失 / trigger others
        Book2DItemClickTrigger trig = target.GetComponent<Book2DItemClickTrigger>();
        if (trig != null)
        {
            if (enableDebugLog) Debug.Log("[Book2DClickManager] -> Book2DItemClickTrigger on " + target.name);
            trig.TriggerFromExternal();
        }
    }

    /// <summary>
    /// 读取 Collider 对象上 SpriteRenderer 的 sortingOrder，没有就当 0
    /// Get sortingOrder from a SpriteRenderer on the collider's GameObject
    /// </summary>
    private int GetSortingOrder(Collider2D col)
    {
        if (col == null) return 0;

        SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            return sr.sortingOrder;
        }

        return 0;
    }
}