using UnityEngine;

/// <summary>
/// 3D 触发器：把“点到 3D 方块”转成对 2D 脚本的调用。
/// - 用鼠标点击 BoxCollider（推荐）
/// - 可选：用 OnTriggerEnter 撞击 Player（现在你先关掉）
/// </summary>
[RequireComponent(typeof(Collider))]
public class Book3DTriggerCall2D : MonoBehaviour
{
    [Header("是否用鼠标点击 3D 区域 / Use mouse click")]
    public bool useMouseClick = true;

    [Header("是否用 3D Trigger Enter 触发 / Use 3D trigger enter")]
    public bool use3DTriggerEnter = false;   // 先关掉，避免进场景就触发

    [Header("3D 碰到的 Player Tag（只在 use3DTriggerEnter=true 时生效）")]
    public string playerTag = "Player";

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("要调用的 2D 脚本 / 2D effect targets")]
    public Book2DItemClickFadeOut[] fadeTargets;
    public Book2DItemClickMoveOnce[] moveOnceTargets;
    public Book2DClickPlayAnimSimple[] animTargets;
    public Book2DItemClickTrigger[] triggerTargets;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private void Reset()
    {
        // 确保 collider 勾上 Is Trigger
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
        if (triggerOnlyOnce && hasTriggered)
        {
            if (enableDebugLog)
                Debug.Log($"[Book3DTriggerCall2D] {name} already triggered, ignore. Reason = {reason}");
            return;
        }

        hasTriggered = true;

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

        // 4) 调用 ItemClickTrigger 里的 UnityEvent（显示 / 隐藏东西）
        if (triggerTargets != null)
        {
            foreach (var t in triggerTargets)
                if (t != null) t.InvokeFromExternal();
        }
    }
}