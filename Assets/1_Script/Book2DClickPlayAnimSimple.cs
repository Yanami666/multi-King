using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DClickPlayAnimSimple : MonoBehaviour
{
    [Header("Animator 设置 / Animator")]
    public Animator animator;            // 不填就自动找本物体上的 Animator
    public string triggerName = "Play";  // 触发用 Trigger 名
    public string stateName = "";        // 如果不用 Trigger，就填一个 State 名来 Play

    [Header("只播放一次 / Play only once")]
    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    [Header("点击后是否禁用 Collider / Disable collider on click")]
    public bool disableColliderOnClick = true;

    [Header("动画播完后是否隐藏自己 / Disable object after animation")]
    public bool disableAfterAnim = false;
    public float extraDisableDelay = 0f;

    [Header("可选 Gate：需要先解锁 / Optional gate to unlock first")]
    public Book2DTriggerGate requiredGate;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private Collider2D _col;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // 一开始禁用 Animator，避免自动播放
        if (animator != null)
            animator.enabled = false;

        _col = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        PlayFromExternal();
    }

    public void PlayFromExternal()
    {
        // 先检查 gate
        if (requiredGate != null && !requiredGate.IsUnlocked)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Gate locked, cannot play: " + name);
            return;
        }

        if (playOnlyOnce && hasPlayed)
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Already played: " + name);
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

        // 现在才真正启用 Animator
        animator.enabled = true;

        // 先用 Trigger
        if (!string.IsNullOrEmpty(triggerName))
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Set Trigger " + triggerName + " on " + name);
            animator.ResetTrigger(triggerName);
            animator.SetTrigger(triggerName);
        }
        else if (!string.IsNullOrEmpty(stateName))
        {
            if (enableDebugLog)
                Debug.Log("[Book2DClickPlayAnimSimple] Play state " + stateName + " on " + name);
            animator.Play(stateName, 0, 0f);
        }

        if (disableAfterAnim)
        {
            float clipLen = 0f;
            if (animator.runtimeAnimatorController != null &&
                animator.runtimeAnimatorController.animationClips != null &&
                animator.runtimeAnimatorController.animationClips.Length > 0)
            {
                clipLen = animator.runtimeAnimatorController.animationClips[0].length;
            }

            float totalDelay = clipLen + extraDisableDelay;
            if (totalDelay <= 0f)
                totalDelay = extraDisableDelay;

            if (totalDelay > 0f)
                StartCoroutine(DisableAfterDelay(totalDelay));
            else
                gameObject.SetActive(false);
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        if (enableDebugLog)
            Debug.Log("[Book2DClickPlayAnimSimple] Disable after " + delay + " seconds: " + name);

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}