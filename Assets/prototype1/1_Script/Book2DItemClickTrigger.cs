using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickTrigger : MonoBehaviour
{
    [Header("要激活的对象 / Objects to activate")]
    public GameObject[] objectsToActivate;

    [Header("要关闭的对象 / Objects to deactivate")]
    public GameObject[] objectsToDeactivate;

    [Header("激活对象时是否渐显 / Fade-in for activated objects")]
    public bool fadeInActivatedObjects = false;
    public float fadeInDuration = 0.5f;

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;

    [Header("触发后是否禁用 Collider / Disable collider after trigger")]
    public bool disableColliderAfterTrigger = true;

    [Header("触发后是否隐藏自己 / Disable this object after trigger")]
    public bool disableSelfAfterTrigger = false;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private bool _hasTriggered = false;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    // 如果不用 ClickManager，可以直接用这个
    private void OnMouseDown()
    {
        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickTrigger] OnMouseDown on " + name);
        }

        TriggerFromExternal();
    }

    // 给 Book2DClickManager 调用
    public void TriggerFromExternal()
    {
        if (triggerOnlyOnce && _hasTriggered)
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DItemClickTrigger] Already triggered on " + name);
            }
            return;
        }

        _hasTriggered = true;

        if (disableColliderAfterTrigger && _col != null)
        {
            _col.enabled = false;
        }

        // 先关闭需要关掉的对象
        if (objectsToDeactivate != null)
        {
            foreach (var go in objectsToDeactivate)
            {
                if (go != null)
                {
                    if (enableDebugLog)
                    {
                        Debug.Log("[Book2DItemClickTrigger] Deactivate " + go.name);
                    }
                    go.SetActive(false);
                }
            }
        }

        // 激活需要激活的对象
        if (objectsToActivate != null && objectsToActivate.Length > 0)
        {
            if (fadeInActivatedObjects && fadeInDuration > 0f)
            {
                StartCoroutine(FadeInObjectsRoutine(objectsToActivate));
            }
            else
            {
                foreach (var go in objectsToActivate)
                {
                    if (go != null)
                    {
                        if (enableDebugLog)
                        {
                            Debug.Log("[Book2DItemClickTrigger] Activate " + go.name);
                        }
                        go.SetActive(true);
                    }
                }
            }
        }

        if (disableSelfAfterTrigger)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeInObjectsRoutine(GameObject[] objs)
    {
        if (objs == null || objs.Length == 0)
            yield break;

        if (enableDebugLog)
        {
            Debug.Log("[Book2DItemClickTrigger] Fade-in activated objects.");
        }

        // 先全部 SetActive(true)，并把 alpha 变成 0
        SpriteRenderer[] srs = new SpriteRenderer[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            GameObject go = objs[i];
            if (go == null)
                continue;

            go.SetActive(true);
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            srs[i] = sr;

            if (sr != null)
            {
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, 0f);
            }
        }

        float t = 0f;
        float duration = Mathf.Max(0.0001f, fadeInDuration);

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            for (int i = 0; i < srs.Length; i++)
            {
                SpriteRenderer sr = srs[i];
                if (sr == null)
                    continue;

                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, lerp);
            }

            yield return null;
        }

        // 最终 alpha = 1
        for (int i = 0; i < srs.Length; i++)
        {
            SpriteRenderer sr = srs[i];
            if (sr == null)
                continue;

            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 1f);
        }
    }
}