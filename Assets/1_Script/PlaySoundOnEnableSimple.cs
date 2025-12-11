using UnityEngine;

/// <summary>
/// 物体被启用时自动播放一次音效。
/// Play a sound when this object becomes enabled (SetActive(true)).
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnEnableSimple : MonoBehaviour
{
    [Header("只播放一次 / Play only once")]
    public bool playOnlyOnce = true;

    private bool hasPlayed = false;

    [Header("可选：覆盖 AudioSource 的 Clip / Optional override clip")]
    public AudioClip overrideClip;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (_source == null)
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        hasPlayed = true;

        // 如果指定了 overrideClip，就用这个，否则用 AudioSource 自己的 clip
        if (overrideClip != null)
        {
            _source.clip = overrideClip;
        }

        if (_source.clip == null)
            return;

        _source.Play();
    }
}