using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DPlayerTriggerShowHide : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    [Header("玩家碰到后要显示的物体 / Objects to ENABLE")]
    public GameObject[] objectsToEnable;

    [Header("玩家碰到后要隐藏的物体 / Objects to DISABLE")]
    public GameObject[] objectsToDisable;

    [Header("是否使用淡入淡出 / Use fade in/out")]
    public bool useFade = false;

    [Header("淡入淡出时间 / Fade duration (seconds)")]
    public float fadeDuration = 0.5f;

    [Header("对子物体里的 SpriteRenderer 也生效 / Affect children")]
    public bool fadeChildrenSprites = true;

    [Header("触发后是否关掉自己 / Disable this trigger after")]
    public bool disableThisTriggerAfter = true;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private void Reset()
    {
        // 自动把 Collider2D 设成 Trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (enableDebugLog)
            Debug.Log("[Book2DPlayerTriggerShowHide] Triggered by " + other.name);

        // 显示
        SetObjectsActive(objectsToEnable, true);

        // 隐藏
        SetObjectsActive(objectsToDisable, false);

        if (disableThisTriggerAfter)
        {
            // 如果在做淡入淡出，先等一会再把自己关掉，避免把协程杀掉
            if (useFade && fadeDuration > 0f)
            {
                StartCoroutine(DisableSelfAfterDelay(fadeDuration));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // 延迟关闭自己
    private IEnumerator DisableSelfAfterDelay(float delay)
    {
        if (enableDebugLog)
            Debug.Log("[Book2DPlayerTriggerShowHide] Will disable trigger after " + delay + " sec");

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // 根据是否使用淡入淡出，统一处理启用/禁用
    private void SetObjectsActive(GameObject[] list, bool active)
    {
        if (list == null) return;

        foreach (var go in list)
        {
            if (go == null) continue;

            if (!useFade || fadeDuration <= 0f)
            {
                // 不用 fade，直接开关
                go.SetActive(active);

                if (enableDebugLog)
                    Debug.Log("[Book2DPlayerTriggerShowHide] SetActive(" + active + ") -> " + go.name);
            }
            else
            {
                if (active)
                {
                    // 淡入
                    StartCoroutine(FadeInObject(go));
                }
                else
                {
                    // 淡出
                    StartCoroutine(FadeOutObject(go));
                }
            }
        }
    }

    // 淡入：先 SetActive(true)，把 alpha 从 0 慢慢拉到原始值
    private IEnumerator FadeInObject(GameObject go)
    {
        if (!go.activeSelf)
            go.SetActive(true);

        var sprites = GetSprites(go);
        if (sprites.Length == 0)
            yield break;

        // 记录目标 alpha，并把当前 alpha 先设为 0
        float[] targetAlphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            Color c = sprites[i].color;
            targetAlphas[i] = c.a <= 0f ? 1f : c.a; // 如果本来就是 0，就当成 1
            c.a = 0f;
            sprites[i].color = c;
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            float k = t / fadeDuration;
            for (int i = 0; i < sprites.Length; i++)
            {
                Color c = sprites[i].color;
                c.a = Mathf.Lerp(0f, targetAlphas[i], k);
                sprites[i].color = c;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // 最终值修正
        for (int i = 0; i < sprites.Length; i++)
        {
            Color c = sprites[i].color;
            c.a = targetAlphas[i];
            sprites[i].color = c;
        }

        if (enableDebugLog)
            Debug.Log("[Book2DPlayerTriggerShowHide] FadeIn done on " + go.name);
    }

    // 淡出：把 alpha 从当前值拉到 0，然后禁用 GameObject
    private IEnumerator FadeOutObject(GameObject go)
    {
        var sprites = GetSprites(go);
        if (sprites.Length == 0)
        {
            go.SetActive(false);
            yield break;
        }

        float[] startAlphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            startAlphas[i] = sprites[i].color.a;
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            float k = t / fadeDuration;
            for (int i = 0; i < sprites.Length; i++)
            {
                Color c = sprites[i].color;
                c.a = Mathf.Lerp(startAlphas[i], 0f, k);
                sprites[i].color = c;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // 最终设为 0，并关掉物体
        for (int i = 0; i < sprites.Length; i++)
        {
            Color c = sprites[i].color;
            c.a = 0f;
            sprites[i].color = c;
        }

        go.SetActive(false);

        if (enableDebugLog)
            Debug.Log("[Book2DPlayerTriggerShowHide] FadeOut done & disabled " + go.name);
    }

    // 取单个或子物体里的 SpriteRenderer
    private SpriteRenderer[] GetSprites(GameObject go)
    {
        if (fadeChildrenSprites)
        {
            return go.GetComponentsInChildren<SpriteRenderer>(true);
        }
        else
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) return new[] { sr };
            return new SpriteRenderer[0];
        }
    }
}