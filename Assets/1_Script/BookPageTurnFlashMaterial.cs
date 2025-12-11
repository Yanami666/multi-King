using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using echo17.EndlessBook;

/// <summary>
/// 在翻页 / 开书动画期间，临时把书的某些材质改成统一的一个材质，
/// 动画结束再改回原来的。
///
/// During page-turn / state-change animation, temporarily replace some book
/// materials with a single "flash" material, then restore the originals.
/// </summary>
public class BookPageTurnFlashMaterial : MonoBehaviour
{
    [Header("EndlessBook 组件 / EndlessBook component")]
    public EndlessBook book;   // 拖有 EndlessBook 的 BookAnimated 上来

    [Header("翻页期间的临时材质 / Flash material during turn")]
    public Material flashMaterial;  // 比如一张纯白/纯黑/纯色材质

    [Header("需要临时替换的材质类型 / Which material types to flash")]
    public EndlessBook.MaterialEnum[] materialTypesToFlash =
    {
        EndlessBook.MaterialEnum.BookPageLeft,
        EndlessBook.MaterialEnum.BookPageRight,
        EndlessBook.MaterialEnum.BookPageFront,
        EndlessBook.MaterialEnum.BookPageBack
    };

    [Header("默认闪动时长（秒）/ Default flash duration (seconds)")]
    public float defaultFlashTime = 0.5f;

    [Header("调试输出 / Debug log")]
    public bool enableDebugLog = false;

    // 存原始材质
    private readonly Dictionary<EndlessBook.MaterialEnum, Material> _originalMaterials =
        new Dictionary<EndlessBook.MaterialEnum, Material>();

    private Coroutine _currentRoutine;

    /// <summary>
    /// 用默认时间闪一下。
    /// Flash materials using defaultFlashTime.
    /// </summary>
    public void FlashNow()
    {
        FlashForDuration(defaultFlashTime);
    }

    /// <summary>
    /// 外部调用：在翻页 / 开书动画开始的那一帧调用。
    /// duration 一般填翻页动画时间（比如 openAnimTime 或 turnTime）。
    ///
    /// Call this when you start a page-turn / state-change animation.
    /// duration should roughly match the animation time.
    /// </summary>
    public void FlashForDuration(float duration)
    {
        if (book == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[BookPageTurnFlashMaterial] Book is NULL.");
            return;
        }

        if (flashMaterial == null)
        {
            if (enableDebugLog)
                Debug.LogWarning("[BookPageTurnFlashMaterial] Flash material is NULL.");
            return;
        }

        if (materialTypesToFlash == null || materialTypesToFlash.Length == 0)
        {
            if (enableDebugLog)
                Debug.LogWarning("[BookPageTurnFlashMaterial] No material types to flash.");
            return;
        }

        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        _currentRoutine = StartCoroutine(FlashRoutine(duration));
    }

    /// <summary>
    /// 具体协程：记录原始材质 -> 替换 -> 等待 -> 还原。
    /// </summary>
    private IEnumerator FlashRoutine(float duration)
    {
        // 1）记录原始材质并替换
        _originalMaterials.Clear();

        foreach (var matType in materialTypesToFlash)
        {
            var original = book.GetMaterial(matType);
            _originalMaterials[matType] = original;

            if (enableDebugLog)
            {
                Debug.Log($"[BookPageTurnFlashMaterial] Set {matType} to flash material.");
            }

            book.SetMaterial(matType, flashMaterial);
        }

        // 2）等动画时间
        if (duration > 0f)
            yield return new WaitForSeconds(duration);
        else
            yield return null; // 至少等一帧

        // 3）还原原始材质
        foreach (var kv in _originalMaterials)
        {
            if (enableDebugLog)
            {
                Debug.Log($"[BookPageTurnFlashMaterial] Restore {kv.Key} to original material.");
            }

            book.SetMaterial(kv.Key, kv.Value);
        }

        _originalMaterials.Clear();
        _currentRoutine = null;
    }
}