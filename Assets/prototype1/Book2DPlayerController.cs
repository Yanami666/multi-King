using UnityEngine;

/// <summary>
/// 控制书页里 2D 小人移动（用 Rigidbody2D.MovePosition，
/// 这样可以被 Collider2D 挡住空气墙）。
/// 2D character controller on the book page using Rigidbody2D.MovePosition,
/// so it collides properly with 2D colliders.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Book2DPlayerController : MonoBehaviour
{
    [Header("移动速度 / Move speed")]
    public float moveSpeed = 3f;

    // 可选：限制活动范围（比如不走出某个矩形）
    public bool useBounds = false;
    public Vector2 minXY = new Vector2(-4f, -3f);
    public Vector2 maxXY = new Vector2(4f, 3f);

    private Rigidbody2D rb;
    private Vector2 inputDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1. 读取输入（只算方向，不在这里改位置）
        // Read input only (don't move here).
        float h = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S, Up/Down

        inputDir = new Vector2(h, v);

        if (inputDir.sqrMagnitude > 1f)
        {
            inputDir = inputDir.normalized;
        }
    }

    private void FixedUpdate()
    {
        // 没输入就不动
        if (inputDir.sqrMagnitude < 0.0001f)
            return;

        // 2. 计算目标位置（用 Rigidbody2D.MovePosition）
        Vector2 currentPos = rb.position;
        Vector2 targetPos = currentPos + inputDir * moveSpeed * Time.fixedDeltaTime;

        // 可选：限制范围（在 MovePosition 之前裁剪）
        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minXY.x, maxXY.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minXY.y, maxXY.y);
        }

        // 3. 用物理移动，这样会受 Collider2D 影响
        rb.MovePosition(targetPos);
    }
}