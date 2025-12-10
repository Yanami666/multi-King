using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickTrigger : MonoBehaviour
{
    [Header("是否允许自己被鼠标点击 / Allow self OnMouseClick")]
    public bool allowSelfClick = true;

    [Header("只触发一次 / Only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("触发后是否禁用自身 Collider / Disable collider after trigger")]
    public bool disableColliderAfterTrigger = true;

    [Header("可选 Gate：需要先解锁 / Optional gate to unlock first")]
    public Book2DTriggerGate requiredGate;

    [Header("被触发后要显示的物体 / Objects to ENABLE after trigger")]
    public GameObject[] objectsToEnable;

    [Header("被触发后要隐藏的物体 / Objects to DISABLE after trigger")]
    public GameObject[] objectsToDisable;

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

        TriggerFromExternal();
    }

    public void TriggerFromExternal()
    {
        // 先检查 gate
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickTrigger] Gate locked, cannot trigger: " + name);
            return;
        }

        if (triggerOnlyOnce && hasTriggered)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickTrigger] Already triggered: " + name);
            return;
        }

        hasTriggered = true;

        if (disableColliderAfterTrigger && _col != null)
            _col.enabled = false;

        if (objectsToEnable != null)
        {
            foreach (var go in objectsToEnable)
            {
                if (go != null)
                    go.SetActive(true);
            }
        }

        if (objectsToDisable != null)
        {
            foreach (var go in objectsToDisable)
            {
                if (go != null)
                    go.SetActive(false);
            }
        }

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickTrigger] Triggered on " + name);
    }
}