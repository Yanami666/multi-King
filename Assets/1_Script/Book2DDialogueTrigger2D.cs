using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Book2DDialogueTrigger2D : MonoBehaviour
{
    [Header("玩家设置 / Player")]
    public string playerTag = "Player";
    public Book2DPlayerController playerController;

    [Header("按顺序显示的对话物体 / Dialogue objects")]
    // 把你场景里已经摆好的图片 GameObject 按顺序拖进来
    public GameObject[] dialogueObjects;

    [Header("这个触发点自己的图标 / Trigger visual")]
    // 触发完之后会隐藏；不填就默认用自己
    public GameObject triggerVisual;

    [Header("输入 / Input")]
    // 左键 + 这个键 都可以切下一张
    public KeyCode advanceKey = KeyCode.Space;

    private bool _hasTriggered = false; // 只触发一次
    private bool _isShowing = false;    // 是否在对话中
    private int _currentIndex = -1;     // 当前显示到第几个

    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;

        if (triggerVisual == null)
            triggerVisual = this.gameObject;

        // 开局先把所有对话物体关掉（你在 Scene 里只负责摆位置）
        HideAllDialogueObjects();
    }

    private void Update()
    {
        if (!_isShowing)
            return;

        // 鼠标左键 或 Space 切下一张
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(advanceKey))
        {
            ShowNextDialogueObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasTriggered)
            return;

        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag))
            return;

        // 找玩家控制脚本
        if (playerController == null)
            playerController = other.GetComponent<Book2DPlayerController>();

        StartDialogueSequence();
    }

    // 开始整套对话
    private void StartDialogueSequence()
    {
        if (dialogueObjects == null || dialogueObjects.Length == 0)
        {
            Debug.LogWarning("[Book2DDialogueTrigger2D] No dialogueObjects configured.");
            return;
        }

        _hasTriggered = true;
        _isShowing = true;
        _currentIndex = -1;

        // 禁用玩家移动
        if (playerController != null)
            playerController.SetCanMove(false);

        ShowNextDialogueObject();
    }

    // 显示下一张
    private void ShowNextDialogueObject()
    {
        // 关掉上一张
        if (_currentIndex >= 0 && _currentIndex < (dialogueObjects?.Length ?? 0))
        {
            if (dialogueObjects[_currentIndex] != null)
                dialogueObjects[_currentIndex].SetActive(false);
        }

        _currentIndex++;

        // 播放完了
        if (dialogueObjects == null || _currentIndex >= dialogueObjects.Length)
        {
            EndDialogueSequence();
            return;
        }

        GameObject go = dialogueObjects[_currentIndex];
        if (go != null)
            go.SetActive(true);
    }

    // 结束对话
    private void EndDialogueSequence()
    {
        _isShowing = false;

        HideAllDialogueObjects();

        if (playerController != null)
            playerController.SetCanMove(true);

        if (triggerVisual != null)
            triggerVisual.SetActive(false);

        if (_col != null)
            _col.enabled = false;
    }

    private void HideAllDialogueObjects()
    {
        if (dialogueObjects == null)
            return;

        foreach (var go in dialogueObjects)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}