using UnityEngine;

/// <summary>
/// 3D 触发器：把“点到 3D collider”转成对 2D 物体脚本的调用。
/// </summary>
[RequireComponent(typeof(Collider))]
public class Book3DTriggerCall2D : MonoBehaviour
{
    [Header("用鼠标点 3D 区域触发 / Use mouse click")]
    public bool useMouseClick = true;

    [Header("用 3D Trigger Enter 触发 / Use 3D trigger enter")]
    public bool use3DTriggerEnter = false;   // 你现在可以先关掉

    [Header("Player 的 Tag（仅在 use3DTriggerEnter=true 时生效）")]
    public string playerTag = "Player";

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool _hasTriggered = false;

    [Header("要调用的 2D 淡出脚本 / Fade out targets")]
    public Book2DItemClickFadeOut[] fadeTargets;

    [Header("要调用的 2D 移动脚本 / Move-once targets")]
    public Book2DItemClickMoveOnce[] moveOnceTargets;

    [Header("要调用的 2D 动画脚本 / Animation targets")]
    public Book2DClickPlayAnimSimple[] animTargets;

    [Header("要调用的 2D Trigger 脚本 / Item trigger targets")]
    public Book2DItemClickTrigger[] triggerTargets;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private void Reset()
    {
        // 方便你一眼看出这是个 trigger
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

        TryTrigger("OnTriggerEnter with " + other.name);
    }

    private void TryTrigger(string reason)
    {
        if (triggerOnlyOnce && _hasTriggered)
        {
            if (enableDebugLog)
                Debug.Log($"[Book3DTriggerCall2D] {name} already triggered, ignore. Reason = {reason}");
            return;
        }

        _hasTriggered = true;

        if (enableDebugLog)
            Debug.Log($"[Book3DTriggerCall2D] TRIGGER {name}. Reason = {reason}");

        // 1) 淡出
        if (fadeTargets != null)
        {
            foreach (var f in fadeTargets)
                if (f != null) f.StartFadeFromExternal();
        }

        // 2) 移动一次
        if (moveOnceTargets != null)
        {
            foreach (var m in moveOnceTargets)
                if (m != null) m.MoveFromExternal();
        }

        // 3) 播放动画
        if (animTargets != null)
        {
            foreach (var a in animTargets)
                if (a != null) a.PlayFromExternal();
        }

        // 4) 触发显示 / 隐藏等
        if (triggerTargets != null)
        {
            foreach (var t in triggerTargets)
                if (t != null) t.InvokeFromExternal();
        }
    }
}