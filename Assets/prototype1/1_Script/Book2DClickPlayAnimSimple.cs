using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DClickPlayAnimSimple : MonoBehaviour
{
    [Header("Animator 设置 / Animator settings")]
    public Animator animator;                 // 不填就自动在本物体上找
    public string triggerName = "Play";       // Animator 里的 Trigger 名
    public string stateName = "";             // 如果不用 Trigger，可以填一个 State 名

    [Header("一开始是否就让 Animator 运行 / Enable animator at start")]
    public bool enableAnimatorOnStart = false;   // 对“云”就把这个勾掉

    [Header("只播放一次 / Play only once")]
    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    [Header("点击后是否禁用 Collider / Disable collider on click")]
    public bool disableColliderOnClick = true;

    [Header("动画播完后是否隐藏自己 / Disable object after animation")]
    public bool disableAfterAnim = false;
    public float extraDisableDelay = 0f;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private Collider2D _col;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        _col = GetComponent<Collider2D>();

        // 🌟 关键：如果你不想开局就播放动画，就把 Animator 先关掉
        if (!enableAnimatorOnStart && animator != null)
        {
            animator.enabled = false;
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Disable animator on start: " + name);
        }

        if (enableDebugLog)
        {
            Debug.Log("[Book2DClickPlayAnimSimple] Awake on " + name +
                      ", animator = " + (animator ? animator.name : "NULL"));
        }
    }

    private void OnMouseDown()
    {
        if (enableDebugLog)
            Debug.Log("[Book2DClickPlayAnimSimple] OnMouseDown on " + name);

        PlayFromExternal();
    }

    // 🔸 3D Trigger 或其它脚本都用这个接口
    public void PlayFromExternal()
    {
        if (playOnlyOnce && hasPlayed)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Already played on " + name);
            return;
        }

        if (animator == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[Book2DClickPlayAnimSimple] Animator is NULL on " + name);
            return;
        }

        hasPlayed = true;

        if (disableColliderOnClick && _col != null)
            _col.enabled = false;

        // 🌟 关键：点到的时候再开启 Animator
        if (!animator.enabled)
            animator.enabled = true;

        // 优先用 Trigger
        if (!string.IsNullOrEmpty(triggerName))
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Set Trigger '" + triggerName + "' on " + name);

            animator.ResetTrigger(triggerName); // 防止残留
            animator.SetTrigger(triggerName);
        }
        // 不用 Trigger 的话，直接 Play 到指定 State
        else if (!string.IsNullOrEmpty(stateName))
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Play State '" + stateName + "' on " + name);

            animator.Play(stateName, 0, 0f);
        }

        if (disableAfterAnim)
        {
            float clipLen = 0f;

            if (animator.runtimeAnimatorController != null &&
                animator.runtimeAnimatorController.animationClips != null &&
                animator.runtimeAnimatorController.animationClips.Length > 0)
            {
                // 简单拿第一个 clip 的长度
                clipLen = animator.runtimeAnimatorController.animationClips[0].length;
            }

            float totalDelay = clipLen + extraDisableDelay;
            if (totalDelay <= 0f)
            {
                totalDelay = extraDisableDelay;
            }

            if (totalDelay > 0f)
                StartCoroutine(DisableAfterDelay(totalDelay));
            else
                gameObject.SetActive(false);
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        if (enableDebugLog)
            Debug.Log("[Book2DClickPlayAnimSimple] Will disable " + name + " after " + delay + " seconds.");

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}