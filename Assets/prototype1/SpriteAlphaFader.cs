using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAlphaFader : MonoBehaviour
{
    public SpriteRenderer targetSprite;

    private void Awake()
    {
        if (targetSprite == null)
            targetSprite = GetComponent<SpriteRenderer>();

        // ★ 启动时强制透明，避免一开场就白屏
        if (targetSprite != null)
        {
            Color c = targetSprite.color;
            c.a = 0f;                 // alpha = 0
            targetSprite.color = c;
        }
    }

    public IEnumerator FadePercent(float fromPercent, float toPercent, float duration)
    {
        if (targetSprite == null)
            yield break;

        float fromAlpha = Mathf.Clamp01(fromPercent / 100f);
        float toAlpha = Mathf.Clamp01(toPercent / 100f);

        Color c = targetSprite.color;
        float t = 0f;

        if (duration <= 0f)
        {
            c.a = toAlpha;
            targetSprite.color = c;
            yield break;
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(fromAlpha, toAlpha, k);
            c.a = a;
            targetSprite.color = c;
            yield return null;
        }

        c.a = toAlpha;
        targetSprite.color = c;
    }
}