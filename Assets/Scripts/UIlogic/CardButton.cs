using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    [SerializeField] private bool isSelected = false;
    [SerializeField] private Image image;
    [Header("按钮图片")]
    [SerializeField] private List<Sprite> sprites;
    [Header("隐藏位置")]
    [SerializeField] private Vector2Int hidePosition;
    [Header("展开位置")]
    [SerializeField] private Vector2Int expandPosition;
    [SerializeField] private CardData cardData;
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        image.sprite = sprites[0];
        // 初始化位置为隐藏位置
        rectTransform.anchoredPosition = hidePosition;
    }
    public void OnClick()
    {
        isSelected = !isSelected;
        image.sprite = isSelected ? sprites[1] : sprites[0];
        if (isSelected)
        {
            GameManager.Instance.SetGameState(GameState.SelectCard);
            ChangePosition(expandPosition);
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.Play);
            ChangePosition(hidePosition);
        }
    }
    void ChangePosition(Vector2Int position)
    {
        StartCoroutine(ChangePositionCoroutine(position));
    }

    IEnumerator ChangePositionCoroutine(Vector2Int position)
    {
        Vector3 targetPosition = new Vector3(position.x, position.y, 0);
        Vector3 startPosition = rectTransform.anchoredPosition;
        float duration = 0.1f; // 0.1秒完成动画
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;



            // 使用Lerp进行平滑插值
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);

            yield return null; // 等待下一帧
        }
        rectTransform.anchoredPosition = targetPosition;
    }
}
