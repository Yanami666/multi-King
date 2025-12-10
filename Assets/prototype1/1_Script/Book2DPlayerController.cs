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
        }
    }
}