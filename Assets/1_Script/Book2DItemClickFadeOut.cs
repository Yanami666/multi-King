using System.Collections;
using UnityEngine;

/// <summary>
/// 点击后让自己从不透明渐变到完全透明（alpha 1 -> 0），
/// 然后可选：关掉 collider、隐藏整 个物体。
/// - 可以自己用 OnMouseDown 触发
/// - 也可以给 Book3DTriggerCall2D 调用 StartFadeFromExternal()
/// - 可选挂一个 Gate，没解锁之前点击无效
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickFadeOut : MonoBehaviour
{
    [Header("是否允许直接点自己 / Allow self OnMouseClick")]
    public bool allowSelfClick = true;

    [Header("只淡出一次 / Fade only once")]
    public bool fadeOnlyOnce = true;

    [Header("淡出时长（秒） / Fade duration (seconds)")]
    public float fadeDuration = 0.5f;

    [Header("淡出时是否禁用 Collider / Disable collider when fading")]
    public bool disableColliderOnStart = true;

    [Header("淡出结束后是否隐藏物体 / Disable object after fade")]
    public bool disableObjectAfterFade = true;

    [Header("可选 Gate：必须先解锁 / Optional gate to require unlock")]
    public Book2DTriggerGate requiredGate;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private bool _hasFaded = false;
    private Coroutine _fadeCo;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        if (!allowSelfClick)
            return;

        StartFadeFromExternal();
    }

    /// <summary>
    /// 提供给外部调用（比如 Book3DTriggerCall2D）
    /// Public entry for external callers.
    /// </summary>
    public void StartFadeFromExternal()
    {
        // 1. 先看 Gate 有没有解锁
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickFadeOut] Gate locked, ignore click. Gate = " + requiredGate.name);
            return;
        }

        // 2. 只允许一次的话，已经淡出过就直接返回
        if (fadeOnlyOnce && _hasFaded)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DItemClickFadeOut] Already faded on " + name);
            return;
        }

        // 3. 正在淡出就不再叠加
        if (_fadeCo != null)
            return;

        _hasFaded = true;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickFadeOut] Start fade on " + name);

        // 开始淡出前先关掉 collider（可选）
        if (disableColliderOnStart && _col != null)
            _col.enabled = false;

        _fadeCo = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (_sr == null)
            yield break;

        Color start = _sr.color;
        Color end = start;
        end.a = 0f;

        float t = 0f;
        float dur = Mathf.Max(0.0001f, fadeDuration);

        while (t < dur)
        {
            float k = t / dur;
            _sr.color = Color.Lerp(start, end, k);
            t += Time.deltaTime;
            yield return null;
        }

        _sr.color = end;

        if (disableObjectAfterFade)
            gameObject.SetActive(false);

        _fadeCo = null;

        if (enableDebugLog)
            Debug.Log("[Book2DItemClickFadeOut] Fade finished on " + name);
    }
}