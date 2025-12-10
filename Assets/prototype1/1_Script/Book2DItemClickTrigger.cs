using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickTrigger : MonoBehaviour
{
    [Header("是否允许自己被鼠标点 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool _hasTriggered = false;

    [Header("触发后关掉自己的 Collider / Disable collider after trigger")]
    public bool disableColliderAfterTrigger = true;

    [Header("触发后隐藏自己 / Disable this object after trigger")]
    public bool disableThisObjectAfterTrigger = false;

    [Header("要改变显示状态的物体 / Objects to show or hide")]
    public GameObject[] targetObjects;

    [Header("触发时设为 Active ? (true=显示,false=隐藏) / SetActive state on trigger")]
    public bool setActiveState = true;

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
    /// 提供给 3D Trigger 调用
    /// </summary>
    public void InvokeFromExternal()
    {
        if (triggerOnlyOnce && _hasTriggered)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickTrigger] Already triggered on " + name);
            return;
        }

        _hasTriggered = true;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickTrigger] Invoke on " + name);

        // 改变目标物体的显示状态
        if (targetObjects != null)
        {
            foreach (var go in targetObjects)
            {
                if (go != null)
                    go.SetActive(setActiveState);
            }
        }

        if (disableColliderAfterTrigger && _col != null)
            _col.enabled = false;

        if (disableThisObjectAfterTrigger)
            gameObject.SetActive(false);
    }
}