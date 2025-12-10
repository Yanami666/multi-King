using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickMoveOnce : MonoBehaviour
{
    [Header("目标位置 / Target position")]
    public Transform target;              // 把你想去的位置做成一个空物体，拖进来
    public bool useLocalPosition = false; // 是否用 localPosition

    [Header("是否平滑移动 / Smooth move")]
    public bool smoothMove = true;
    public float moveTime = 0.5f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("触发一次后是否关掉 Collider / Disable collider after move")]
    public bool disableColliderAfterMove = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private bool _hasMoved = false;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    // 如果不用 ClickManager，可以直接用这个
    private void OnMouseDown()
    {
        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickMoveOnce] OnMouseDown on " + name);
        }

        OnExternalClick();
    }

    // 给 Book2DClickManager 调用
    public void OnExternalClick()
    {
        if (_hasMoved)
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DItemClickMoveOnce] Already moved on " + name);
            }
            return;
        }

        if (target == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("[Book2DItemClickMoveOnce] Target is NULL on " + name);
            }
            return;
        }

        _hasMoved = true;

        if (disableColliderAfterMove && _col != null)
        {
            _col.enabled = false;
        }

        if (smoothMove && moveTime > 0f)
        {
            StartCoroutine(MoveRoutine());
        }
        else
        {
            if (useLocalPosition)
                transform.localPosition = target.localPosition;
            else
                transform.position = target.position;
        }
    }

    private IEnumerator MoveRoutine()
    {
        Vector3 start = useLocalPosition ? transform.localPosition : transform.position;
        Vector3 end = useLocalPosition ? target.localPosition : target.position;

        float t = 0f;
        float duration = Mathf.Max(0.0001f, moveTime);

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickMoveOnce] Start moving " + name + " to " + end);
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            float curve = moveCurve != null ? moveCurve.Evaluate(lerp) : lerp;
            Vector3 pos = Vector3.Lerp(start, end, curve);

            if (useLocalPosition)
                transform.localPosition = pos;
            else
                transform.position = pos;

            yield return null;
        }

        if (useLocalPosition)
            transform.localPosition = end;
        else
            transform.position = end;

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickMoveOnce] Move finished on " + name);
        }
    }
}