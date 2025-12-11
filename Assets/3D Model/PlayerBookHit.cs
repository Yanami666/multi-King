using UnityEngine;

public class PlayerBookHit : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 看看撞到的物体上有没有 BookCoverSwitcher
        BookCoverSwitcher book = hit.collider.GetComponent<BookCoverSwitcher>();
        if (book != null)
        {
            book.SwitchCover();
        }
    }
}