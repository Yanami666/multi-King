using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickFadeOut : MonoBehaviour
{
    [Header("是否允许直接点 2D 自己 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("只淡出一次 / Fade only once")]
    public bool fadeOnlyOnce = true;

    [Header("淡出时长（秒） / Fade duration (seconds)")]
    public float fadeDuration = 0.5f;

    [Header("点击后立刻关掉 collider / Disable collider on click")]
    public bool disableColliderOnClick = true;

    [Header("淡出结束后是否隐藏物体 / Disable object after fade")]
    public bool disableObjectAfterFade = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private Color _startColor;
    private bool _isFading = false;
    private bool _hasFaded = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _startColor = _sr.color;
    }

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickFadeOut] OnMouseDown on " + name);

        StartFadeFromExternal();
    }

    /// <summary>
    /// 提供给外部调用：比如 Book3DTriggerCall2D
    /// Public entry for 3D trigger, etc.
    /// </summary>
    public void StartFadeFromExternal()
    {
        if (fadeOnlyOnce && _hasFaded)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickFadeOut] Already faded on " + name);
            return;
        }

        if (_isFading)
            return;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        _isFading = true;
        _hasFaded = true;

        if (disableColliderOnClick && _col != null)
            _col.enabled = false;

        float t = 0f;
        Color c = _startColor;

        while (t < fadeDuration)
        {
            float a = Mathf.Lerp(_startColor.a, 0f, t / fadeDuration);
            _sr.color = new Color(c.r, c.g, c.b, a);

            t += Time.deltaTime;
            yield return null;
        }

        _sr.color = new Color(c.r, c.g, c.b, 0f);

        if (disableObjectAfterFade)
            gameObject.SetActive(false);

        _isFading = false;
    }
}