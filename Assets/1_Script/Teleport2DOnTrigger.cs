using UnityEngine;

/// <summary>
/// 当 2D 玩家进入这个 Trigger 时，把玩家传送到指定位置，顺便播放一个音效（可选）。
/// Teleport the 2D player to a target point and play a sound.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Teleport2DOnTrigger : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("传送目标点 / Teleport target point")]
    public Transform targetPoint;

    [Header("音效设置 / Sound settings (optional)")]
    [Tooltip("如果勾选，优先在玩家身上的 AudioSource 播放")]
    public bool playOnPlayer = true;

    [Tooltip("要用来播放传送音效的 AudioSource（可空）")]
    public AudioSource audioSource;

    [Tooltip("要播放的音效 Clip（可空，如果 AudioSource 里已经有就可以不填）")]
    public AudioClip teleportClip;

    private void Reset()
    {
        // 自动把 Collider2D 设成 Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 只对玩家生效
        if (!other.CompareTag(playerTag))
            return;

        if (targetPoint == null)
        {
            Debug.LogWarning("[Teleport2DOnTrigger] targetPoint is not set.");
            return;
        }

        // --- 1. 先传送玩家 ---
        Transform playerTrans = other.transform;
        Vector3 pos = targetPoint.position;
        pos.z = playerTrans.position.z;   // 保持原来的 Z
        playerTrans.position = pos;

        // --- 2. 再播放音效（如果有设置） ---
        PlayTeleportSound(other.gameObject);
    }

    private void PlayTeleportSound(GameObject playerObject)
    {
        // 没有任何音源和 clip 就直接返回
        if (!playOnPlayer && audioSource == null && teleportClip == null)
            return;

        AudioSource src = null;

        if (playOnPlayer)
        {
            // 优先在玩家身上找 AudioSource
            src = playerObject.GetComponent<AudioSource>();
        }

        // 如果玩家身上没有 AudioSource，就退回到 Inspector 上指定的 audioSource
        if (src == null)
        {
            src = audioSource;
        }

        if (src == null)
            return;

        // 如果指定了 clip，就用这个；否则用 AudioSource 本身的 clip
        if (teleportClip != null)
        {
            src.PlayOneShot(teleportClip);
        }
        else
        {
            src.Play();
        }
    }
}