using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickFadeOut : MonoBehaviour
{
    [Header("自身渐隐 / Fade out self")]
    public float fadeDuration = 1f;           // 自己从 1 -> 0 的时间
    public bool disableColliderOnClick = true;
    public bool disableObjectAfterFade = true;

    [Header("淡出后要出现的物体（可选） / Object to show after fade")]
    public GameObject objectToShow;           // 这里可以拖一个新的物体
    public bool showWithFadeIn = false;       // 是否给新物体做 0 -> 1 渐显
    public float fadeInDuration = 0.5f;       // 新物体渐显时间

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private SpriteRenderer _sprite;
    private Collider2D _collider;
    private bool _isFading = false;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (_sprite == null)
        {
            Debug.LogError("[Book2DItemClickFadeOut] SpriteRenderer missing on " + name);
        }

        if (_collider == null)
        {
            Debug.LogError("[Book2DItemClickFadeOut] Collider2D missing on " + name);
        }
    }

    // 如果你不用统一的 ClickManager，可以直接用这个
    private void OnMouseDown()
    {
        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickFadeOut] OnMouseDown on " + name);
        }

        StartFadeFromExternal();
    }

    // 给 Book2DClickManager 调用的入口
    public void StartFadeFromExternal()
    {
        if (_isFading)
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DItemClickFadeOut] Already fading on " + name);
            }
            return;
        }

        if (_sprite == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("[Book2DItemClickFadeOut] Sprite is NULL on " + name);
            }
            return;
        }

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        _isFading = true;

        if (disableColliderOnClick && _collider != null)
        {
            _collider.enabled = false;
        }

        Color c = _sprite.color;
        float startAlpha = c.a;
        float endAlpha = 0f;
        float t = 0f;
        float duration = Mathf.Max(0.0001f, fadeDuration);

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickFadeOut] Fade-out start on " + name);
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(startAlpha, endAlpha, lerp);
            _sprite.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        // 确保完全透明
        _sprite.color = new Color(c.r, c.g, c.b, endAlpha);

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickFadeOut] Fade-out finished on " + name);
        }

        // 处理要出现的新物体
        if (objectToShow != null)
        {
            objectToShow.SetActive(true);

            if (showWithFadeIn)
            {
                SpriteRenderer showSprite = objectToShow.GetComponent<SpriteRenderer>();
                if (showSprite != null)
                {
                    yield return StartCoroutine(FadeInNewObject(showSprite));
                }
            }
        }

        if (disableObjectAfterFade)
        {
            gameObject.SetActive(false);
        }

        _isFading = false;
    }

    private IEnumerator FadeInNewObject(SpriteRenderer targetSprite)
    {
        if (targetSprite == null)
            yield break;

        Color c = targetSprite.color;
        float finalAlpha = c.a <= 0f ? 1f : c.a;   // 如果原来是 0，就淡到 1
        float startAlpha = 0f;
        float t = 0f;
        float duration = Mathf.Max(0.0001f, fadeInDuration);

        // 先把 alpha 置 0
        targetSprite.color = new Color(c.r, c.g, c.b, startAlpha);

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickFadeOut] Fade-in object " + targetSprite.name);
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(startAlpha, finalAlpha, lerp);
            targetSprite.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        targetSprite.color = new Color(c.r, c.g, c.b, finalAlpha);
    }
}