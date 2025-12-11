using System.Collections;
using UnityEngine;

/// <summary>
/// 控制一个 SpriteRenderer 的 alpha（0~1）
/// 只给白色遮罩用：SetAlpha 直接设透明度，FadePercent 做 0-100% 渐变。
///
/// Controls a SpriteRenderer's alpha (0-1).
/// Designed for the white overlay sprite only.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaFader : MonoBehaviour
{
    [Header("要渐变的 Sprite / Target sprite")]
    public SpriteRenderer targetSprite;

    private void Awake()
    {
        if (targetSprite == null)
            targetSprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 直接设置 alpha（0~1）
    /// Instantly set alpha (0-1).
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (targetSprite == null)
            return;

        alpha = Mathf.Clamp01(alpha);
        var c = targetSprite.color;
        c.a = alpha;
        targetSprite.color = c;
    }

    /// <summary>
    /// 以百分比 0~100 渐变。需要用 StartCoroutine 调用。
    /// Fade from percent (0-100) to percent (0-100). Call via StartCoroutine.
    /// </summary>
    public IEnumerator FadePercent(float fromPercent, float toPercent, float duration)
    {
        if (targetSprite == null)
            yield break;

        float fromAlpha = Mathf.Clamp01(fromPercent / 100f);
        float toAlpha = Mathf.Clamp01(toPercent / 100f);

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