using UnityEngine;

/// <summary>
/// 当 2D 玩家走到书页里的某块区域时，显示/隐藏翻页 UI。
/// Show/hide the EndlessBook UI when the 2D player enters a trigger zone in the book page.
/// 挂在带 BoxCollider2D(IsTrigger) 的触发区上。
/// Attach this to a GameObject with a BoxCollider2D marked as IsTrigger.
/// </summary>
public class BookPageUITrigger2D : MonoBehaviour
{
    [Header("要显示/隐藏的 UI 根物体 / UI root to toggle")]
    public GameObject uiRoot;

    [Header("玩家的 Tag / Player tag")]
    public string playerTag = "Player";

    private void Start()
    {
        // 开局隐藏 UI（防止一开始是开着的）
        // Hide UI at start just in case.
        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && uiRoot != null)
        {
            uiRoot.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && uiRoot != null)
        {
            uiRoot.SetActive(false);
        }
    }
}