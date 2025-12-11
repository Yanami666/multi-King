using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Exit2DFadeToScene : MonoBehaviour
{
    [Header("玩家 Tag / Player tag")]
    public string playerTag = "Player";

    [Header("要切换到的场景名 / Scene to load")]
    public string sceneName;

    [Header("黑屏遮罩 / Black overlay fader")]
    public SpriteAlphaFader fadeOverlay;     // 拖那个全屏黑色遮罩的 SpriteAlphaFader

    [Header("淡入黑屏时间 / Fade to black time")]
    public float fadeTime = 1.0f;

    [Header("只触发一次 / Trigger only once")]
    public bool triggerOnlyOnce = true;
    private bool hasTriggered = false;

    private void Reset()
    {
        // 自动把 Collider2D 设成 Trigger
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;
        StartCoroutine(CoFadeAndLoad());
    }

    private IEnumerator CoFadeAndLoad()
    {
        // 1）全屏黑色渐入
        if (fadeOverlay != null)
        {
            // 0% -> 100% 黑
            yield return fadeOverlay.FadePercent(0f, 100f, fadeTime);
        }

        // 2）切换场景（此时画面已经是纯黑，看不到切换）
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("[Exit2DFadeToScene] sceneName is empty.");
        }
    }
}