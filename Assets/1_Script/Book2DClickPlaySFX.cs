using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))]
public class Book2DClickPlaySFX : MonoBehaviour
{
    [Header("是否允许直接点 2D 自己 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只播放一次？ / Play only once?")]
    public bool playOnlyOnce = false;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private AudioSource _audio;
    private bool _hasPlayed = false;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        {
            Debug.LogWarning("[Book2DClickPlaySFX] No AudioSource on " + name);
        }
    }

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        if (enableDebugLog)
            Debug.Log("[Book2DClickPlaySFX] OnMouseDown on " + name);

        PlayFromExternal();
    }

    public void PlayFromExternal()
    {
        if (playOnlyOnce && _hasPlayed)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlaySFX] Already played on " + name);
            return;
        }

        if (_audio == null || _audio.clip == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[Book2DClickPlaySFX] No clip on " + name);
            return;
        }

        _audio.Play(); // 或者用 PlayOneShot(_audio.clip)
        _hasPlayed = true;

        if (enableDebugLog)
            Debug.Log("[Book2DClickPlaySFX] Play SFX on " + name);
    }
}