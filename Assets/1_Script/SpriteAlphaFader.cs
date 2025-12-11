using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 控制一个 SpriteRenderer 的 alpha 渐变（0~1）
/// 可用：
/// - SetAlpha() 直接设定透明度
/// - FadeTo() 从当前 alpha 渐变到目标 alpha
/// - FadePercent() 用 0~100 百分比去渐变
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

        // 默认：一开始完全透明（你这块图是用来当遮罩的）
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

        SetAlpha(a);

        if (t >= 1f)
        {
            _isFading = false;
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }

    /// <summary>
    /// 立刻设置 alpha，不做渐变。
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

    /// <summary>
    /// 从当前 alpha 渐变到目标 alpha（0~1）。
    /// </summary>
    public void FadeTo(float targetAlpha, float duration, Action onComplete = null)
    {
        if (targetSprite == null)
            return;

        _startAlpha = targetSprite.color.a;
        _targetAlpha = Mathf.Clamp01(targetAlpha);
        _duration = duration;
        _timer = 0f;
        _onComplete = onComplete;
        _isFading = true;
    }

    /// <summary>
    /// 使用 0~100% 控制渐变，比如从 100 到 0 = 从不透明到透明。
    /// </summary>
    public IEnumerator FadePercent(float fromPercent, float toPercent, float duration)
    {
        if (targetSprite == null)
            yield break;

        float fromAlpha = Mathf.Clamp01(fromPercent / 100f);
        float toAlpha = Mathf.Clamp01(toPercent / 100f);

        // 起点
        SetAlpha(fromAlpha);

        if (duration <= 0f)
        {
            SetAlpha(toAlpha);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(fromAlpha, toAlpha, k);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(toAlpha);
    }
}