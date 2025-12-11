using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstep2D : MonoBehaviour
{
    [Header("移动参考 / Movement source (可选)")]
    [Tooltip("如果有 Rigidbody2D 就拖进来；留空则用 Transform 位移算速度")]
    public Rigidbody2D movementRb;

    [Header("脚步声音源 / Footstep audio source")]
    public AudioSource footstepSource;

    [Header("判定为“在走路”的最小速度 / Min speed to count as walking")]
    public float minSpeed = 0.1f;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    // 用 Transform 算速度时用
    private Vector3 _lastPosition;

    private void Reset()
    {
        // 自动找组件
        if (movementRb == null)
            movementRb = GetComponentInParent<Rigidbody2D>();

        if (footstepSource == null)
            footstepSource = GetComponent<AudioSource>();

        if (footstepSource != null)
        {
            // 脚步声一般是 Loop，且不要 Play On Awake
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
    }

    private void Awake()
    {
        if (_lastPosition == Vector3.zero)
            _lastPosition = transform.position;

        // 防止你在 Inspector 里忘记关 PlayOnAwake
        if (footstepSource != null)
            footstepSource.playOnAwake = false;
    }

    private void Update()
    {
        if (footstepSource == null)
            return;

        float speed = GetCurrentSpeed();

        if (enableDebugLog)
        {
            Debug.Log($"[PlayerFootstep2D] speed = {speed:F3}, " +
                      $"isPlaying = {footstepSource.isPlaying}");
        }

        // 速度大于阈值 -> 确保在播放
        if (speed >= minSpeed)
        {
            if (!footstepSource.isPlaying && footstepSource.clip != null)
            {
                footstepSource.Play();
            }
        }
        // 速度太小 -> 停止
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    /// <summary>
    /// 先用 Rigidbody2D 的速度；如果没设置，就用 Transform 位移算。
    /// </summary>
    private float GetCurrentSpeed()
    {
        if (movementRb != null)
        {
            return movementRb.linearVelocity.magnitude;
        }

        // 没有 Rigidbody2D 的情况，用位移/时间算速度
        Vector3 currentPos = transform.position;
        float distance = (currentPos - _lastPosition).magnitude;
        float speed = distance / Mathf.Max(Time.deltaTime, 0.0001f);
        _lastPosition = currentPos;
        return speed;
    }
}