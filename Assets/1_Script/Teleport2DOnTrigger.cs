using UnityEngine;

public class Teleport2DOnTrigger : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("传送目标点 / Teleport target point")]
    public Transform targetPoint;

    [Header("可选 Gate：需要先解锁 / Optional gate to unlock first")]
    public Book2DTriggerGate requiredGate;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (requiredGate != null && !requiredGate.IsUnlocked)
            return;

        if (targetPoint == null)
        {
            Debug.LogWarning("[Teleport2DOnTrigger] targetPoint is not set.");
            return;
        }

        Transform playerTrans = other.transform;
        Vector3 pos = targetPoint.position;
        pos.z = playerTrans.position.z;
        playerTrans.position = pos;
    }
}