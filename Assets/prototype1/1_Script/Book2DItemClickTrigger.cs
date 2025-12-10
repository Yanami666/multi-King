using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DItemClickTrigger : MonoBehaviour
{
    // 这个物体上的 Animator（云的动画）
    public Animator animator;

    // Animator 里用的 Trigger 参数名，例如 "Play"
    public string triggerName = "Play";

    // 要隐藏的物体，默认就是自己
    public GameObject targetToHide;

    // 动画播完多少秒后隐藏
    public float hideDelay = 0.5f;

    private bool _activated = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (targetToHide == null)
            targetToHide = gameObject;
    }

    // 鼠标点击时触发
    private void OnMouseDown()
    {
        if (_activated)
            return;

        _activated = true;

        if (animator != null && !string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }

        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        if (hideDelay > 0f)
            yield return new WaitForSeconds(hideDelay);

        if (targetToHide != null)
            targetToHide.SetActive(false);
    }
}