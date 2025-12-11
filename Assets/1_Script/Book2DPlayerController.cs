using UnityEngine;

public class Book2DPlayerController : MonoBehaviour
{
    [Header("Movement / 移动")]
    public float moveSpeed = 3f;

    // 是否限制活动范围
    public bool useBounds = false;
    public Vector2 minXY = new Vector2(-4f, -3f);
    public Vector2 maxXY = new Vector2(4f, 3f);

    // 对话时会被关掉
    [Header("Can Move 开关（对话时禁用移动）")]
    public bool canMove = true;

    public void SetCanMove(bool value)
    {
        canMove = value;

        // 一旦禁止移动，立刻停脚步声
        if (!canMove)
        {
            StopFootstep();
        }
    }

    // ---------------- 动画相关字段 ----------------
    [Header("Walking Animation / 走路动画")]
    public SpriteRenderer spriteRenderer;   // 主角身上的 SpriteRenderer

    public Sprite idleSprite;               // 站立帧（人物本身的 sprite）
    public Sprite[] walkFrames;             // 走路帧（4 张朝右）
    public float animationFPS = 8f;         // 动画帧率可调

    private int currentFrame = 0;
    private float frameTimer = 0f;
    // ----------------------------------------------------

    // ---------------- 脚步声音相关字段 ----------------
    [Header("Footstep Audio / 脚步声音")]
    [Tooltip("拖入带有 walking-footstep 的 AudioSource（Loop=On, PlayOnAwake=Off）")]
    public AudioSource footstepSource;

    [Tooltip("多小的输入算是“在走路”")]
    public float inputThreshold = 0.1f;
    // ----------------------------------------------------

    private void Update()
    {
        if (!canMove)
            return;

        // 1. 读方向输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, v, 0f);

        // 用输入大小来判断是否在“走路”
        bool isMoving = dir.sqrMagnitude > inputThreshold * inputThreshold;

        if (isMoving)
        {
            dir = dir.normalized;

            // 2. 移动
            Vector3 pos = transform.position;
            pos += dir * moveSpeed * Time.deltaTime;

            if (useBounds)
            {
                pos.x = Mathf.Clamp(pos.x, minXY.x, maxXY.x);
                pos.y = Mathf.Clamp(pos.y, minXY.y, maxXY.y);
            }

            transform.position = pos;

            // 3. 左右翻转
            if (h > 0.01f)
                spriteRenderer.flipX = false;   // 朝右
            else if (h < -0.01f)
                spriteRenderer.flipX = true;    // 朝左

            // 4. 播放走路动画
            PlayWalkAnimation();

            // 5. 确保脚步声在播
            StartFootstep();
        }
        else
        {
            // 不动：显示站立帧
            if (idleSprite != null)
            {
                spriteRenderer.sprite = idleSprite;
            }

            // 重置动画
            currentFrame = 0;
            frameTimer = 0f;

            // 停脚步声
            StopFootstep();
        }
    }

    // ---------------- 播放走路动画 ----------------
    private void PlayWalkAnimation()
    {
        if (spriteRenderer == null)
            return;

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

    // ---------------- 脚步声音控制 ----------------
    private void StartFootstep()
    {
        if (footstepSource == null)
            return;

        // 没在播 + 有 clip 的时候才 Play()
        if (!footstepSource.isPlaying && footstepSource.clip != null)
        {
            footstepSource.Play();
        }
    }

    private void StopFootstep()
    {
        if (footstepSource == null)
            return;

        if (footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }
    // ---------------------------------------------------
}