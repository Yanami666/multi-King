using UnityEngine;

/// <summary>
/// 简单的“锁 / Gate”脚本：
/// - 可以设置一开始是否解锁
/// - 其他脚本通过 IsUnlocked 查询状态
/// - 通过 Unlock() 把门打开
/// </summary>
public class Book2DTriggerGate : MonoBehaviour
{
    [Header("Gate 名字（方便在 Console 看）")]
    public string gateName = "Gate";

    [Header("一开始就解锁？ / Start unlocked?")]
    public bool startUnlocked = false;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = true;

    private bool _isUnlocked = false;

    /// <summary>当前是否已解锁</summary>
    public bool IsUnlocked => _isUnlocked;

    private void Awake()
    {
        _isUnlocked = startUnlocked;

        if (enableDebugLog)
        {
            Debug.Log($"[Book2DTriggerGate] {gateName} Awake. StartUnlocked = {_isUnlocked}");
        }
    }

    /// <summary>外部调用：解锁</summary>
    public void Unlock()
    {
        if (_isUnlocked)
        {
            if (enableDebugLog)
                Debug.Log($"[Book2DTriggerGate] {gateName} already unlocked.");
            return;
        }

        _isUnlocked = true;

        if (enableDebugLog)
            Debug.Log($"[Book2DTriggerGate] UNLOCKED: {gateName}");
    }
}