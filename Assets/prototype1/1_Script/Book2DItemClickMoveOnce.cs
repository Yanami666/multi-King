using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickMoveOnce : MonoBehaviour
{
    // 是否使用目标 Transform 来决定新位置（更直观，用空物体当锚点）
    public bool useTargetTransform = true;

    // 目标位置（如果 useTargetTransform = false，就用这个 Vector3）
    public Vector3 targetPosition;

    // 目标 Transform（比如放一个空物体在你想要的位置）
    public Transform targetTransform;

    // 是否在本地坐标系下移动（一般建议 false，用世界坐标）
    public bool useLocalPosition = false;

    private bool _activated = false;

    private void Awake()
    {
        // 确保有 Collider，可以被 OnMouseDown 检测到
        var col = GetComponent<Collider2D>();
        col.isTrigger = false; // 点按不一定非要 trigger
    }

    private void OnMouseDown()
    {
        if (_activated)
            return;

        _activated = true;

        Vector3 newPos;

        if (useTargetTransform && targetTransform != null)
        {
            newPos = targetTransform.position;
        }
        else
        {
            newPos = targetPosition;
        }

        if (useLocalPosition)
        {
            transform.localPosition = newPos;
        }
        else
        {
            transform.position = newPos;
        }

        // 单次触发：移动完后禁用脚本，防止再次点击
        enabled = false;
    }
}