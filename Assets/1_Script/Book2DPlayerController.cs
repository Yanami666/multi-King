using UnityEngine;

public class Book2DPlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;

    // 是否限制活动范围
    public bool useBounds = false;
    public Vector2 minXY = new Vector2(-4f, -3f);
    public Vector2 maxXY = new Vector2(4f, 3f);

    // 对话时会被关掉
    public bool canMove = true;

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    // ---------------- 新增：动画相关字段 ----------------
    [Header("Walking Animation")]
    public SpriteRenderer spriteRenderer;   // 主角身上的 SpriteRenderer

    public Sprite idleSprite;               // 站立帧（人物本身的 sprite）
    public Sprite[] walkFrames;             // 走路帧（4 张朝右）
    public float animationFPS = 8f;         // 动画帧率可调

    private int currentFrame = 0;
    private float frameTimer = 0f;
    // ----------------------------------------------------

    private void Update()
    {
        if (!canMove)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, v, 0f);

        if (dir.sqrMagnitude > 0.0001f)
        {
            dir = dir.normalized;

            Vector3 pos = transform.position;
            pos += dir * moveSpeed * Time.deltaTime;

            if (useBounds)
            {
                pos.x = Mathf.Clamp(pos.x, minXY.x, maxXY.x);
                pos.y = Mathf.Clamp(pos.y, minXY.y, maxXY.y);
            }

            transform.position = pos;

            // ----------- 左右翻转 ----------
            if (h > 0.01f)
                spriteRenderer.flipX = false;   // 朝右
            else if (h < -0.01f)
                spriteRenderer.flipX = true;    // 朝左
            // --------------------------------

            // ----------- 播放走路动画 ----------
            PlayWalkAnimation();
            // ----------------------------------
        }
        else
        {
            // ----------- 不动：显示站立帧 ----------
            if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }

            // 重置动画
            currentFrame = 0;
            frameTimer = 0f;
        }
    }

    // ---------------- 新增：播放走路动画 ----------------
    private void PlayWalkAnimation()
    {
        if (walkFrames == null || walkFrames.Length == 0)
            return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / animationFPS)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % walkFrames.Length;
            spriteRenderer.sprite = walkFrames[currentFrame];
        }
    }
    // ---------------------------------------------------
}