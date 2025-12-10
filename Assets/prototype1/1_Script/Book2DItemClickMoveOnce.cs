using System.Collections;
using UnityEngine;

/// <summary>
/// 把 2D 物体从当前位置移动到指定目标（一次性）。
/// 可以自己点，也可以被 3D trigger 调用 MoveOnceFromExternal()。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickMoveOnce : MonoBehaviour
{
    [Header("移动目标 / Move target")]
    public Transform targetTransform;       // 直接拖一个空物体作目标
    public bool useLocalPosition = false;

    [Header("时间 / Duration")]
    public float moveDuration = 0.5f;

    [Header("是否允许自己用鼠标点 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只执行一次 / Only once")]
    public bool moveOnlyOnce = true;
    private bool hasMoved = false;

    [Header("移动后是否禁用 collider / Disable collider after move")]
    public bool disableColliderAfterMove = true;

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
            Debug.Log("[Book2DItemClickMoveOnce] OnMouseDown on " + name);

        MoveOnceFromExternal();
    }

    /// <summary>提供给外部调用</summary>
    public void MoveOnceFromExternal()
    {
        if (moveOnlyOnce && hasMoved)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickMoveOnce] Already moved on " + name);
            return;
        }

        if (targetTransform == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[Book2DItemClickMoveOnce] targetTransform is NULL on " + name);
            return;
        }

        hasMoved = true;
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        if (enableDebugLog)
            Debug.Log("[Book2DItemClickMoveOnce] Start move on " + name);

        Vector3 start = useLocalPosition ? transform.localPosition : transform.position;
        Vector3 end = useLocalPosition ? targetTransform.localPosition : targetTransform.position;

        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / moveDuration);
            Vector3 pos = Vector3.Lerp(start, end, k);

            if (useLocalPosition)
                transform.localPosition = pos;
            else
                transform.position = pos;

            yield return null;
        }

        if (disableColliderAfterMove && _col != null)
            _col.enabled = false;
    }
}