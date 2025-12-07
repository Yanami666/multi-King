using System;
using UnityEngine;

/// <summary>
/// 控制一个 SpriteRenderer 的 alpha 渐变（0~1）
/// 自己在 Update 里跑，不再用外面协程。
///
/// Controls the alpha of a SpriteRenderer from 0 to 1
/// using an internal state machine with optional onComplete callback.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaFader : MonoBehaviour
{
    [Header("要渐变的 Sprite / Target sprite")]
    public SpriteRenderer targetSprite;

    private bool _isFading = false;
    private float _startAlpha;
    private float _targetAlpha;
    private float _duration;
    private float _timer;
    private Action _onComplete;

    private void Awake()
    {
        if (targetSprite == null)
            targetSprite = GetComponent<SpriteRenderer>();

        // 启动时强制透明，避免一开场白屏
        if (targetSprite != null)
        {
            Color c = targetSprite.color;
            c.a = 0f;
            targetSprite.color = c;
        }
    }

    private void Update()
    {
        if (!_isFading || targetSprite == null)
            return;

        _timer += Time.deltaTime;
        float t = _duration <= 0f ? 1f : Mathf.Clamp01(_timer / _duration);
        float a = Mathf.Lerp(_startAlpha, _targetAlpha, t);

        Color c = targetSprite.color;
        c.a = a;
        targetSprite.color = c;

        if (t >= 1f)
        {
            _isFading = false;
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }

    /// <summary>
    /// 开始渐变到指定 alpha（0~1）
    /// Start fading to target alpha (0~1).
    /// </summary>
    public void FadeTo(float targetAlpha, float duration, Action onComplete = null)
    {
        if (targetSprite == null)
            return;

        Color c = targetSprite.color;
        _startAlpha = c.a;
        _targetAlpha = Mathf.Clamp01(targetAlpha);
        _duration = duration;
        _timer = 0f;
        _onComplete = onComplete;
        _isFading = true;
    }

    /// <summary>
    /// 立刻设定 alpha，不渐变。
    /// Instantly set alpha without fading.
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (targetSprite == null)
            return;

        alpha = Mathf.Clamp01(alpha);
        Color c = targetSprite.color;
        c.a = alpha;
        targetSprite.color = c;
        _isFading = false;
        _onComplete = null;
    }
}