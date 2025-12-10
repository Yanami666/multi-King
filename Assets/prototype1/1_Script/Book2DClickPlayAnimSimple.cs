using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DClickPlayAnimSimple : MonoBehaviour
{
    [Header("Animator 设置 / Animator")]
    public Animator animator;          // 不填就自动在本物体上找

    [Header("点击后才允许 Animator 运行 / Lock animator until click")]
    public bool lockAnimatorAtStart = true;

    [Header("触发方式 / How to play")]
    public string triggerName = "Play";   // 用 Trigger 的方式（推荐，和 Animator 里的 Trigger 对应）
    public string stateName = "";         // 如果不用 Trigger，可以填 State 名直接 Play（triggerName 为空时才用）

    [Header("只播放一次 / Play only once")]
    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    [Header("点击后是否禁用 Collider / Disable collider on click")]
    public bool disableColliderOnClick = false;

    [Header("动画结束后是否隐藏自己 / Disable object after animation")]
    public bool disableAfterAnim = false;
    public float extraDisableDelay = 0f;  // 动画结束后再多等一点时间

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    private Collider2D _col;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        _col = GetComponent<Collider2D>();

        // 一开始把 Animator 锁住（不运行），等点击之后再打开
        if (lockAnimatorAtStart && animator != null)
        {
            animator.enabled = false;
        }

        if (enableDebugLog)
        {
            Debug.Log("[Book2DClickPlayAnimSimple] Awake on " + name +
                      ", animator = " + (animator ? animator.name : "NULL"));
        }
    }

    // 直接用 Unity 自带 OnMouseDown：在 Game 视图中点到这个物体就会触发
    private void OnMouseDown()
    {
        if (enableDebugLog)
        {
            Debug.Log("[Book2DClickPlayAnimSimple] OnMouseDown on " + name);
        }

        PlayFromExternal();
    }

    // 给外部（比如你之后的 ClickManager）也可以用的入口
    public void PlayFromExternal()
    {
        if (playOnlyOnce && hasPlayed)
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DClickPlayAnimSimple] Already played on " + name);
            }
            return;
        }

        if (animator == null)
        {
            if (enableDebugLog)
            {
                Debug.LogWarning("[Book2DClickPlayAnimSimple] Animator is NULL on " + name);
            }
            return;
        }

        hasPlayed = true;

        // 之前锁住的话，这里打开 Animator，让它开始跑状态机
        if (lockAnimatorAtStart && !animator.enabled)
        {
            animator.enabled = true;
        }

        // 点击后不想再被点就关掉 collider
        if (disableColliderOnClick && _col != null)
        {
            _col.enabled = false;
        }

        // 优先用 Trigger 方式
        if (!string.IsNullOrEmpty(triggerName))
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DClickPlayAnimSimple] Set Trigger '" + triggerName + "' on " + name);
            }

            animator.ResetTrigger(triggerName);
            animator.SetTrigger(triggerName);
        }
        // 如果 triggerName 为空，就用 Play(stateName) 方式
        else if (!string.IsNullOrEmpty(stateName))
        {
            if (enableDebugLog)
            {
                Debug.Log("[Book2DClickPlayAnimSimple] Play State '" + stateName + "' on " + name);
            }

            animator.Play(stateName, 0, 0f);
        }

        // 播完之后整个物体隐藏
        if (disableAfterAnim)
        {
            float clipLen = 0f;

            if (animator.runtimeAnimatorController != null &&
                animator.runtimeAnimatorController.animationClips != null &&
                animator.runtimeAnimatorController.animationClips.Length > 0)
            {
                // 简单用第一个动画片段长度作为参考
                clipLen = animator.runtimeAnimatorController.animationClips[0].length;
            }

            float totalDelay = clipLen + extraDisableDelay;
            if (totalDelay <= 0f)
            {
                totalDelay = extraDisableDelay;
            }

            if (totalDelay > 0f)
            {
                StartCoroutine(DisableAfterDelay(totalDelay));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        if (enableDebugLog)
        {
            Debug.Log("[Book2DClickPlayAnimSimple] Will disable " + name + " after " + delay + " seconds.");
        }

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}