using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class CardLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CardData cardData;
    [SerializeField] private int useCount;//使用次数
    [SerializeField] private TextMeshProUGUI useCountText;//使用次数文本
    [SerializeField] private Image useImage;//使用图片
    [SerializeField] private Image cardDetail;//卡牌图片
    [SerializeField] private TextMeshProUGUI cardDetailText;//卡牌详情文本
    [SerializeField] private TextMeshProUGUI cardNameText;//卡牌详情文本2
    // 拖拽相关变量
    private bool isMouseOver = false; // 鼠标是否悬停
    [SerializeField] private bool isDragging = false; // 是否正在拖拽
    [SerializeField] private Vector3 originalPosition; // 原始位置（在GridLayout中的位置）
    [SerializeField] private Vector3 offset; // 鼠标点击位置与物体中心的偏移
    private RectTransform rectTransform; // RectTransform组件
    private Canvas parentCanvas; // 父级Canvas
    private GridLayoutGroup parentGridLayout; // 父级GridLayout组件
    private Transform originalParent; // 原始父级（GridLayout容器）
    private Transform dragParent; // 拖拽时的父级（通常是Canvas）
    private Vector3 currentPosition; // 当前位置（在Canvas中的位置）

    // 纵坐标判断阈值
    [SerializeField] private float useCardThreshold = 100f; // 触发UseCard方法的纵坐标阈值

    void Start()
    {
        // 获取必要的组件
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        parentGridLayout = GetComponentInParent<GridLayoutGroup>();
        originalParent = transform.parent;

        // 设置拖拽时的父级（通常是Canvas的直接子级）
        if (parentCanvas != null)
        {
            GameObject dragParentObj = new GameObject("DragParent");
            dragParent = dragParentObj.transform;
            dragParent.SetParent(parentCanvas.transform);
            dragParentObj.AddComponent<RectTransform>();
        }

        // 记录原始位置
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // 如果正在拖拽，更新物体位置
        if (isDragging)
        {
            UpdateDragPosition();
        }
    }
    // 鼠标悬停进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        cardDetail.gameObject.SetActive(true);
        cardDetailText.text = cardData.getCardDescription;
        cardNameText.text = cardData.getCardName;

        OnMouseSelected();

    }

    // 鼠标悬停离开
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        cardDetail.gameObject.SetActive(false);
        OnMouseDeselected();


    }

    // 鼠标按下（开始拖拽）
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            cardDetail.gameObject.SetActive(false);
            StartDrag(eventData);
        }
    }

    // 鼠标释放（结束拖拽）
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isDragging)
        {
            EndDrag();
        }
    }
    /// <summary>
    /// 开始拖拽 - 修复版本
    /// </summary>
    void StartDrag(PointerEventData eventData)
    {
        isDragging = true;

        // 记录原始位置
        originalPosition = rectTransform.anchoredPosition;

        // 更精确的偏移计算
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Overlay模式：直接使用屏幕坐标计算偏移
            offset = rectTransform.position - (Vector3)eventData.position;
        }
        else
        {
            // Camera模式：使用局部坐标计算偏移
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                offset = rectTransform.anchoredPosition - localPoint;
            }
            else
            {
                // 备用方案：使用屏幕坐标
                offset = rectTransform.position - (Vector3)eventData.position;
            }
        }

        // 切换到拖拽父级，避免受GridLayout影响
        if (dragParent != null)
        {
            transform.SetParent(dragParent);
        }

        // 设置到最上层
        transform.SetAsLastSibling();

    }
    /// <summary>
    /// 鼠标悬停时调用的方法
    /// </summary>
    void OnMouseSelected()
    {
        // 悬停效果：放大10%
        transform.localScale = Vector3.one * 1.1f;
    }

    /// <summary>
    /// 鼠标离开时调用的方法
    /// </summary>
    void OnMouseDeselected()
    {
        // 如果不是正在拖拽，恢复原始大小
        if (!isDragging)
        {
            transform.localScale = Vector3.one;
        }

    }
    /// <summary>
    /// 更新拖拽位置 - 修复版本
    /// </summary>
    void UpdateDragPosition()
    {
        if (!isDragging) return;

        // 方法1：直接使用鼠标屏幕坐标（最简单有效）
        Vector3 mouseScreenPos = Input.mousePosition;

        // 根据Canvas渲染模式选择正确的坐标转换方式
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // Screen Space - Overlay 模式：直接使用屏幕坐标
            rectTransform.position = mouseScreenPos + offset;
            currentPosition = rectTransform.anchoredPosition;
        }
        else
        {
            // Screen Space - Camera 模式：使用摄像机坐标转换
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mouseScreenPos,
                parentCanvas.worldCamera,
                out localPoint))
            {
                rectTransform.anchoredPosition = localPoint + (Vector2)offset;
            }
            else
            {
                // 备用方案：直接使用屏幕坐标
                rectTransform.position = mouseScreenPos + offset;
            }
            currentPosition = rectTransform.anchoredPosition;
        }


    }
    /// <summary>
    /// 结束拖拽
    /// </summary>
    void EndDrag()
    {
        isDragging = false;
        // 检查纵坐标是否超过阈值
        CheckVerticalPosition();

        // 回到GridLayout容器并重新排列
        ReturnToGridLayout();

        // 恢复原始大小
        transform.localScale = Vector3.one;

    }

    /// <summary>
    /// 检查纵坐标并决定是否执行UseCard方法
    /// </summary>
    void CheckVerticalPosition()
    {
        // 获取当前世界坐标的Y值
        float currentY = currentPosition.y;

        // 获取原始位置的世界坐标Y值（作为参考点）
        float originalY = originalPosition.y;

        // 计算与原始位置的垂直距离
        float verticalDistance = Mathf.Abs(currentY - originalY);


        // 如果垂直距离超过阈值，执行UseCard方法
        if (verticalDistance > useCardThreshold)
        {
            UseCard();
        }
    }


    void UseCard()
    {

        EventHandler.CallPlayerUseSkill(cardData);


    }

    /// <summary>
    /// 回到GridLayout容器并重新排列
    /// </summary>
    void ReturnToGridLayout()
    {
        // 回到原始父级
        transform.SetParent(originalParent);

        // 平滑回到原始位置
        StartCoroutine(SmoothReturnToGrid());
    }

    /// <summary>
    /// 平滑回到GridLayout中的位置
    /// </summary>
    IEnumerator SmoothReturnToGrid()
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        float duration = 0.3f; // 返回动画时长
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 使用缓动函数使动画更平滑
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, originalPosition, progress);

            yield return null;
        }

        // 确保最终位置准确
        rectTransform.anchoredPosition = originalPosition;

        // 强制刷新GridLayout（确保正确排列）
        if (parentGridLayout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentGridLayout.GetComponent<RectTransform>());
        }
    }

    /// <summary>
    /// 强制停止拖拽（外部调用）
    /// </summary>
    public void ForceStopDrag()
    {
        if (isDragging)
        {
            EndDrag();
        }
    }

    /// <summary>
    /// 更新原始位置（当GridLayout发生变化时调用）
    /// </summary>
    public void UpdateOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    // 原有的方法保持不变
    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
    }

    public void SetUseCount(int count)
    {
        useCount = count;
        useCountText.text = useCount.ToString();
    }

    public void SetUseImage()
    {
        useImage.sprite = cardData.getCardSprite;
    }

    // 公共属性访问
    public bool IsDragging => isDragging;
    public bool IsMouseOver => isMouseOver;
    public Vector3 OriginalPosition => originalPosition;
    public CardData GetCardData => cardData;
}