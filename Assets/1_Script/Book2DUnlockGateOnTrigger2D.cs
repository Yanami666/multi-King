using UnityEngine;

/// <summary>
/// 玩家进入 2D Trigger 时，解锁一个 Gate。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Book2DUnlockGateOnTrigger2D : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("要被解锁的 Gate / Gate to unlock")]
    public Book2DTriggerGate gateToUnlock;

    [Header("这个解锁动作本身是否也需要先有 Gate？（可选）")]
    public Book2DTriggerGate requiredGate;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = true;

    private void Reset()
    {
        // 确保是 Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        // 如果自己也需要某个 Gate 先解锁
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log($"[Book2DUnlockGateOnTrigger2D] Required gate LOCKED ({requiredGate.gateName}), ignore.");
            return;
        }

        if (gateToUnlock == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[Book2DUnlockGateOnTrigger2D] gateToUnlock is NULL.");
            return;
        }

        gateToUnlock.Unlock();

        if (enableDebugLog)
            Debug.Log($"[Book2DUnlockGateOnTrigger2D] Player hit trigger, UNLOCK {gateToUnlock.gateName}.");
    }
}