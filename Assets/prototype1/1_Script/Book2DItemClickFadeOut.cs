using System.Collections;
using UnityEngine;

/// <summary>
/// 让 Sprite 渐隐（依赖 SpriteAlphaFader）。
/// 可以：
/// - 自己 OnMouseDown 触发
/// - 被 Book3DTriggerCall2D 调用 StartFadeFromExternal()
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Book2DItemClickFadeOut : MonoBehaviour
{
    [Header("渐隐设置 / Fade settings")]
    public float fadeDuration = 1f;
    public bool disableObjectAfterFade = true;
    public bool disableColliderOnStart = true;

    [Header("是否允许自己用鼠标点 / Allow self OnMouseClick")]
    public bool allowSelfClick = false;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private SpriteAlphaFader _fader;
    private Collider2D _col;
    private bool _hasFaded = false;

    private void Awake()
    {
        _fader = GetComponent<SpriteAlphaFader>();
        if (_fader == null)
        {
            _fader = gameObject.AddComponent<SpriteAlphaFader>();
        }

        _col = GetComponent<Collider2D>();
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
    /// 提供给外部（3D trigger 或别的脚本）调用
    /// </summary>
    public void StartFadeFromExternal()
    {
        if (_hasFaded)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickFadeOut] Already faded on " + name);
            return;
        }

        _hasFaded = true;

        if (disableColliderOnStart && _col != null)
            _col.enabled = false;

        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (enableDebugLog)
            Debug.Log("[Book2DItemClickFadeOut] Start fade on " + name);

        // 默认从 100% -> 0%
        yield return _fader.FadePercent(100f, 0f, fadeDuration);

        if (disableObjectAfterFade)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickFadeOut] Disable object " + name);
            gameObject.SetActive(false);
        }
    }
}