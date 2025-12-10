using System.Diagnostics;
using System.Numerics;
using UnityEngine;

/// <summary>
/// 当 2D 玩家进入这个 Trigger 时，把玩家传送到指定位置。
/// Teleport the 2D player to a target point when entering this trigger.
/// </summary>
public class Teleport2DOnTrigger : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("传送目标点 / Teleport target point")]
    public Transform targetPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只对玩家生效
        // Only react to the player
        if (!other.CompareTag(playerTag))
            return;

        if (targetPoint == null)
        {
            UnityEngine.Debug.LogWarning("[Teleport2DOnTrigger] targetPoint is not set.");
            return;
        }

        // 传送玩家到目标点（保持 z 不变）
        // Teleport player, keep its original Z
        Transform playerTrans = other.transform;
        UnityEngine.Vector3 pos = targetPoint.position;
        pos.z = playerTrans.position.z;
        playerTrans.position = pos;
    }
}