using UnityEngine;

public class BookCoverSwitcher : MonoBehaviour
{
    public Material normalMat;
    public Material touchedMat;

    private Renderer rend;
    private bool changed = false;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && normalMat != null)
        {
            rend.material = normalMat;
        }
    }

    // 由外部（Player）调用
    public void SwitchCover()
    {
        if (changed) return;

        if (rend == null)
            rend = GetComponent<Renderer>();

        if (rend != null && touchedMat != null)
        {
            rend.material = touchedMat;
            changed = true;
        }
    }
}