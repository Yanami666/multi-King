using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 管理“每一页对应一个 BGM”
/// One global AudioSource, different clips per spread/page.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BookBGMPerSpread : MonoBehaviour
{
    [Header("每页对应的 BGM / BGM per spread")]
    [Tooltip("0 可以当封面，1=第一页，2=第二页... 顺序随你")]
    public AudioClip[] spreadBGMs;

    [Header("淡入淡出时间 / Fade time (seconds)")]
    public float fadeTime = 1f;

    private AudioSource _source;
    private int _currentIndex = -1;
    private Coroutine _fadeRoutine;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;       // 背景音乐一般循环
        _source.playOnAwake = false;
    }

    /// <summary>
    /// 对外调用：根据“页索引”播放对应 BGM
    /// spreadIndex = 0/1/2/3 对应 spreadBGMs 数组里的元素
    /// </summary>
    public void PlayForSpread(int spreadIndex)
    {
        if (spreadBGMs == null || spreadBGMs.Length == 0)
            return;

        if (spreadIndex < 0 || spreadIndex >= spreadBGMs.Length)
        {
            UnityEngine.Debug.LogWarning($"[BookBGMPerSpread] spreadIndex {spreadIndex} out of range.");
            return;
        }

        // 同一首就不用切
        if (spreadIndex == _currentIndex && _source.isPlaying)
            return;

        AudioClip newClip = spreadBGMs[spreadIndex];
        if (newClip == null)
        {
            UnityEngine.Debug.LogWarning($"[BookBGMPerSpread] BGM clip is NULL at index {spreadIndex}.");
            return;
        }

        _currentIndex = spreadIndex;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeToClip(newClip));
    }

    /// <summary>
    /// 可选：直接播某个 clip（比如封面打开时特定音乐）
    /// </summary>
    public void PlayClip(AudioClip clip)
    {
        if (clip == null)
            return;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeToClip(clip));
    }

    private IEnumerator FadeToClip(AudioClip newClip)
    {
        if (fadeTime <= 0.01f)
        {
            _source.clip = newClip;
            _source.volume = 1f;
            _source.Play();
            yield break;
        }

        float startVol = _source.isPlaying ? _source.volume : 0f;
        float t = 0f;

        // 1）先淡出现在这首
        while (t < fadeTime)
        {
            float k = t / fadeTime;
            _source.volume = Mathf.Lerp(startVol, 0f, k);
            t += Time.deltaTime;
            yield return null;
        }

        _source.volume = 0f;
        _source.clip = newClip;
        _source.Play();

        // 2）再淡入新的一首
        t = 0f;
        while (t < fadeTime)
        {
            float k = t / fadeTime;
            _source.volume = Mathf.Lerp(0f, 1f, k);
            t += Time.deltaTime;
            yield return null;
        }

        _source.volume = 1f;
        _fadeRoutine = null;
    }
}