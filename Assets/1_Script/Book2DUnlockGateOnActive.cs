using UnityEngine;

public class Book2DUnlockGateOnActive : MonoBehaviour
{
    [Header("要解锁的 Gate / Gate to unlock")]
    public Book2DTriggerGate gate;

    [Header("被监控的物体（SetActive(true) 就解锁） / Watched object")]
    public GameObject targetObject;

    [Header("只解锁一次？ / Only unlock once?")]
    public bool onlyOnce = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private bool _hasUnlocked = false;

    private void Reset()
    {
        // 如果这个脚本和 Gate 挂在同一个物体上，就自动引用它
        if (gate == null)
            gate = GetComponent<Book2DTriggerGate>();
    }

    private void Update()
    {
        if (gate == null || targetObject == null)
            return;

        if (onlyOnce && _hasUnlocked)
            return;

        // activeInHierarchy = 本体和所有父物体都 Active
        if (targetObject.activeInHierarchy)
        {
            gate.Unlock();
            _hasUnlocked = true;

            if (enableDebugLog)
            {
                Debug.Log($"[Book2DUnlockGateOnActive] Target '{targetObject.name}' is active, unlock gate '{gate.gateName}'.");
            }

            // 解锁一次就够了，可以把脚本关掉避免每帧检测
            if (onlyOnce)
                enabled = false;
        }
    }
}