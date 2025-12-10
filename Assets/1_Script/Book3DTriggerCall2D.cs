using UnityEngine;

/// <summary>
/// 3D Collider 上的点击 / 碰撞 -> 去调用 2D 物体上的脚本。
/// - 可选：需要某个 Gate 解锁之后才允许触发
/// - 会按顺序触发：动画 / 渐隐 / 移动 / 触发器 / 音效
/// </summary>
[RequireComponent(typeof(Collider))]
public class Book3DTriggerCall2D : MonoBehaviour
{
    [Header("触发方式 / How to trigger")]
    public bool useMouseClick = true;        // 用鼠标点这个 3D Collider
    public bool use3DTriggerEnter = false;   // 或者用 3D Trigger 撞击
    public string playerTag = "Player";      // 只有 Player 撞到才算

    [Header("只触发一次 / Only trigger once")]
    public bool triggerOnlyOnce = true;
    private bool _hasTriggered = false;

    [Header("（可选）需要的门锁 Gate / Optional gate")]
    public Book2DTriggerGate requiredGate;   // 不填就代表不用锁

    [Header("要去调用的 2D 行为 / 2D behaviours to call")]
    public Book2DClickPlayAnimSimple anim2D;     // 播放动画
    public Book2DItemClickFadeOut fade2D;        // 渐隐
    public Book2DItemClickMoveOnce move2D;       // 移动一次
    public Book2DItemClickTrigger trigger2D;     // 显示/隐藏其它物体
    public Book2DClickPlaySFX sfx2D;             // ✅ 新增：播放音效

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = true;

    private void Reset()
    {
        // 默认改成 Trigger，方便你用 OnTriggerEnter
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnMouseDown()
    {
        if (!useMouseClick)
            return;

        TryTrigger("OnMouseDown");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!use3DTriggerEnter)
            return;

        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        TryTrigger("OnTriggerEnter");
    }

    /// <summary>
    /// 真正的触发逻辑集中在这里
    /// </summary>
    private void TryTrigger(string fromWhere)
    {
        // 1. 先检查 Gate：锁没开就直接 return
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
            {
                Debug.Log($"[Book3DTriggerCall2D] Gate LOCKED, ignore. From={fromWhere}, Gate={requiredGate.name}");
            }
            return;
        }

        // 2. 再检查“只触发一次”
        if (triggerOnlyOnce && _hasTriggered)
        {
            if (enableDebugLog)
            {
                Debug.Log($"[Book3DTriggerCall2D] Already triggered on {name}, ignore. From={fromWhere}");
            }
            return;
        }

        _hasTriggered = true;

        if (enableDebugLog)
        {
            Debug.Log($"[Book3DTriggerCall2D] TRIGGER from {fromWhere} on {name}");
        }

        // 3. 按顺序触发各个 2D 行为（有就调用，没有就跳过）

        if (anim2D != null)
        {
            if (enableDebugLog) Debug.Log("[Book3DTriggerCall2D] PlayFromExternal -> " + anim2D.name);
            anim2D.PlayFromExternal();
        }

        if (fade2D != null)
        {
            if (enableDebugLog) Debug.Log("[Book3DTriggerCall2D] StartFadeFromExternal -> " + fade2D.name);
            fade2D.StartFadeFromExternal();
        }

        if (move2D != null)
        {
            if (enableDebugLog) Debug.Log("[Book3DTriggerCall2D] MoveFromExternal -> " + move2D.name);
            move2D.MoveFromExternal();
        }

        if (trigger2D != null)
        {
            if (enableDebugLog) Debug.Log("[Book3DTriggerCall2D] TriggerFromExternal -> " + trigger2D.name);
            trigger2D.TriggerFromExternal();
        }

        if (sfx2D != null)
        {
            if (enableDebugLog) Debug.Log("[Book3DTriggerCall2D] Play SFX -> " + sfx2D.name);
            sfx2D.PlayFromExternal();
        }
    }
}