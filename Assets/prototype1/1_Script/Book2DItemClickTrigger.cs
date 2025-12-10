using UnityEngine;

/// <summary>
/// 一个非常单纯的“点了就让一些物体出现/消失”的脚本。
/// - 可以自己 OnMouseDown 触发（2D collider）
/// - 也可以被 3D Trigger 调用 InvokeFromExternal()
/// - 不再用 UnityEvent，只是 SetActive(...)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickTrigger : MonoBehaviour
{
    [Header("是否允许自己用鼠标点 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("触发后是否禁用自己的 2D Collider / Disable collider after trigger")]
    public bool disableColliderAfterTrigger = true;

    [Header("触发后是否隐藏自己这个物体 / Hide this object after trigger")]
    public bool disableThisObjectAfterTrigger = false;

    [Header("要改变显隐状态的物体 / Objects to show or hide")]
    public GameObject[] targetObjects;

    [Header("把它们改成什么状态 / SetActive state")]
    public bool setActiveState = true; // 通常选 true = 让它们出现

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickTrigger] OnMouseDown on " + name);

        InvokeFromExternal();
    }

    /// <summary>
    /// 提供给外部调用（比如 Book3DTriggerCall2D）
    /// </summary>
    public void InvokeFromExternal()
    {
        if (triggerOnlyOnce && hasTriggered)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickTrigger] Already triggered on " + name);
            return;
        }

        hasTriggered = true;

        // 1. 改变目标物体的显隐状态
        if (targetObjects != null)
        {
            foreach (var go in targetObjects)
            {
                if (go == null) continue;

                if (enableDebugLog)
                    Debug.Log($"[Book2DItemClickTrigger] SetActive({setActiveState}) on {go.name}");

                go.SetActive(setActiveState);
            }
        }

        // 2. 关掉自己的 collider
        if (disableColliderAfterTrigger && _col != null)
            _col.enabled = false;

        // 3. 看需不需要直接隐藏自己
        if (disableThisObjectAfterTrigger)
            gameObject.SetActive(false);
    }
}