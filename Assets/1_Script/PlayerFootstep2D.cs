using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootstep2D : MonoBehaviour
{
    [Header("移动参考（推荐 Rigidbody2D） / Movement source")]
    public Rigidbody2D rb;              // 如果你有自己的移动脚本，也可以不用 rb

    [Header("脚步声音源 / Footstep audio")]
    public AudioSource footstepSource;  // 循环的脚步声音 AudioSource

    [Header("判定为移动的最小速度 / Min speed to be 'walking'")]
    public float minSpeed = 0.1f;

    private void Reset()
    {
        // 自动帮你抓同一个物体上的组件
        rb = GetComponent<Rigidbody2D>();
        footstepSource = GetComponent<AudioSource>();

        if (footstepSource != null)
        {
            footstepSource.loop = true;      // 确保循环
            footstepSource.playOnAwake = false; // 不要一开始就播放
        }
    }

    private void Update()
    {
        bool isMoving = false;

        // 如果有 Rigidbody2D，就用速度判断
        if (rb != null)
        {
            isMoving = rb.linearVelocity.sqrMagnitude > (minSpeed * minSpeed);
        }

        // 根据是否在移动，控制脚步声
        if (isMoving)
        {
            if (footstepSource != null && !footstepSource.isPlaying && footstepSource.clip != null)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource != null && footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }
}