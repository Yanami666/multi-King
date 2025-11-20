using UnityEngine;

/// <summary>
/// 控制书页里 2D 小人移动的脚本（基于 X/Y 平面）。
/// Simple 2D character controller for the book page (X/Y plane).
/// </summary>
public class Book2DPlayerController : MonoBehaviour
{
    [Header("移动速度 / Move Speed")]
    public float moveSpeed = 3f;

    // 可选：限制活动范围（比如不走出画面）
    // Optional: clamp movement inside a rectangle
    public bool useBounds = false;
    public Vector2 minXY = new Vector2(-4f, -3f);
    public Vector2 maxXY = new Vector2(4f, 3f);

    void Update()
    {
        // 1. 读取输入 Read input (WASD / Arrow keys)
        float h = Input.GetAxisRaw("Horizontal"); // A/D, Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S, Up/Down

        Vector3 dir = new Vector3(h, v, 0f);

        // 如果没有按键就不动 Only move if there is some input
        if (dir.sqrMagnitude > 0.0001f)
        {
            dir = dir.normalized;

            // 2. 计算移动 Move the character
            Vector3 pos = transform.position;
            pos += dir * moveSpeed * Time.deltaTime;

            // 3. 限制范围 Clamp to a rectangle (optional)
            if (useBounds)
            {
                pos.x = Mathf.Clamp(pos.x, minXY.x, maxXY.x);
                pos.y = Mathf.Clamp(pos.y, minXY.y, maxXY.y);
            }

            // 4. 应用位置 Apply position
            transform.position = pos;
        }
    }
}