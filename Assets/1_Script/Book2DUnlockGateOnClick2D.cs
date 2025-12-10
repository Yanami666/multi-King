using UnityEngine;

/// <summary>
/// 鼠标点这个 2D 物体时，解锁一个 Gate。
/// 可以用在云、按钮、图标等需要“点击开锁”的情况。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Book2DUnlockGateOnClick2D : MonoBehaviour
{
    [Header("是否允许直接用 OnMouseDown 点击自己")]
    public bool allowSelfClick = true;

    [Header("要被解锁的 Gate / Gate to unlock")]
    public Book2DTriggerGate gateToUnlock;

    [Header("这个解锁动作本身是否也需要先有 Gate？（可选）")]
    public Book2DTriggerGate requiredGate;

    [Header("只解锁一次 / Only unlock once")]
    public bool unlockOnlyOnce = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = true;

    private bool _hasUnlocked = false;

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        TryUnlock();
    }

    /// <summary>提供给 3D Trigger 或别的脚本调用</summary>
    public void UnlockFromExternal()
    {
        TryUnlock();
    }

    private void TryUnlock()
    {
        if (unlockOnlyOnce && _hasUnlocked)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DUnlockGateOnClick2D] Already unlocked once on " + name);
            return;
        }

        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log($"[Book2DUnlockGateOnClick2D] Required gate LOCKED ({requiredGate.gateName}), ignore.");
            return;
        }

        if (gateToUnlock == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[Book2DUnlockGateOnClick2D] gateToUnlock is NULL.");
            return;
        }

        gateToUnlock.Unlock();
        _hasUnlocked = true;

        if (enableDebugLog)
            Debug.Log($"[Book2DUnlockGateOnClick2D] Click -> UNLOCK {gateToUnlock.gateName} on {name}");
    }
}