using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickMoveOnce : MonoBehaviour
{
    [Header("是否允许直接点 2D 自己 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只移动一次 / Move only once")]
    public bool moveOnlyOnce = true;

    [Header("移动时使用本地坐标 / Use local position")]
    public bool useLocalPosition = true;

    [Header("目标位置（如果 targetTransform 不为空，将用它） / Target position")]
    public Vector3 targetPosition;

    [Header("可以直接拖一个空物体当目标点 / Optional target transform")]
    public Transform targetTransform;

    [Header("移动时长（0 = 瞬移） / Move duration (0 = instant)")]
    public float moveDuration = 0.3f;

    [Header("点击后是否关掉 collider / Disable collider on click")]
    public bool disableColliderOnClick = true;

    [Header("可选 Gate：需要先解锁 / Optional gate to unlock first")]
    public Book2DTriggerGate requiredGate;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private Collider2D _col;
    private bool _hasMoved = false;
    private bool _isMoving = false;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        MoveFromExternal();
    }

    public void MoveFromExternal()
    {
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickMoveOnce] Gate locked, cannot move: " + name);
            return;
        }

        if (moveOnlyOnce && _hasMoved)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickMoveOnce] Already moved: " + name);
            return;
        }

        if (_isMoving)
            return;

        _hasMoved = true;

        if (disableColliderOnClick && _col != null)
            _col.enabled = false;

        // 如果设置了空物体，就用它的位置
        if (targetTransform != null)
        {
            targetPosition = useLocalPosition ? targetTransform.localPosition : targetTransform.position;
        }

        if (moveDuration <= 0f)
        {
            ApplyPosition(targetPosition);
        }
        else
        {
            StartCoroutine(MoveRoutine());
        }
    }

    private void ApplyPosition(Vector3 target)
    {
        if (useLocalPosition)
            transform.localPosition = target;
        else
            transform.position = target;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickMoveOnce] Moved " + name + " to " + target);
    }

    private IEnumerator MoveRoutine()
    {
        _isMoving = true;

        Vector3 startPos = useLocalPosition ? transform.localPosition : transform.position;
        Vector3 endPos = targetPosition;

        float t = 0f;

        while (t < moveDuration)
        {
            float k = moveDuration > 0f ? t / moveDuration : 1f;
            Vector3 p = Vector3.Lerp(startPos, endPos, k);

            if (useLocalPosition)
                transform.localPosition = p;
            else
                transform.position = p;

            t += Time.deltaTime;
            yield return null;
        }

        ApplyPosition(endPos);
        _isMoving = false;
    }
}