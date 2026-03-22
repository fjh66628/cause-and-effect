using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardData;//卡牌数据
    [SerializeField] private bool isClicked = false;//是否点击了对话框
    void OnEnable()
    {
        EventHandler.updateCard += SetCardData;
        EventHandler.showDialogue += HaveDialogue;
    }
    void OnDisable()
    {
        EventHandler.updateCard -= SetCardData;
        EventHandler.showDialogue -= HaveDialogue;
    }
    [Header("卡牌")]
    [SerializeField] private GameObject card;//卡牌图片
    [SerializeField] private GameObject dialogueContainer;//对话框
    private Dictionary<string, GameObject> createdCardInstances = new Dictionary<string, GameObject>();

    void Start()
    {

    }

    void HaveDialogue(DialogueSO dialogueData)
    {
        DialogueShow();
        // 显示对话内容
        EventHandler.onMouseLeftClick += CheckClicked;
        TextMeshProUGUI dialogueText = dialogueContainer.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(ShowDialogue(dialogueText, dialogueData));
        EventHandler.onMouseLeftClick -= CheckClicked;
        DialogueHide();
    }

    IEnumerator ShowDialogue(TextMeshProUGUI dialogueText, DialogueSO dialogueData)
    {
        foreach (var dialogue in dialogueData.getDialogues)
        {
            // 显示对话内容
            dialogueText.text = dialogue.getDialogue;
            yield return WaitForInput();
        }
    }
    void CheckClicked(Vector2 position)//检查是否点击了对话框
    {
        isClicked = true;
    }

    IEnumerator WaitForInput()
    {
        while (!isClicked)
        {

            yield return null;
        }
        isClicked = false;
        yield return new WaitForSeconds(0.5f);
    }

    void DialogueShow()
    {
        dialogueContainer.SetActive(true);
    }
    void DialogueHide()
    {
        dialogueContainer.SetActive(false);
    }

    public void SetCardData()//设置传入卡牌数据
    {
        StartCoroutine(UpdateCardData());//更新传牌数据

    }
    IEnumerator UpdateCardData()//更新传牌数据 
    {
        yield return null;
        this.cardData = GameManager.Instance.GetCardData();//传入卡牌数据赋值 
        GameObject targetSceneRoot = GameObject.Find($"卡牌容器"); // 找到目标场景的根节点 

        // 清理不存在的卡牌数据对应的实例
        CleanupOrphanedInstances();

        foreach (var item in cardData)
        {
            // 检查是否已经创建过这个卡牌的实例
            if (!createdCardInstances.ContainsKey(item.getCardName))
            {
                CreateCardImage(item, targetSceneRoot);
            }
            else
            {
                // 如果已经存在，更新现有实例而不是创建新的
                UpdateExistingCardImage(item, createdCardInstances[item.getCardName]);
            }
        }
    }

    void CreateCardImage(CardData cardData, GameObject parent)
    {
        // 创建新的卡牌UI实例
        GameObject cardInstance = Instantiate(card, parent.transform);

        // 设置卡牌数据
        SetupCardUI(cardInstance, cardData);

        // 添加到管理字典
        createdCardInstances[cardData.getCardName] = cardInstance;

    }

    void UpdateExistingCardImage(CardData cardData, GameObject existingInstance)
    {
        // 更新现有实例的数据
        SetupCardUI(existingInstance, cardData);
    }

    void SetupCardUI(GameObject cardInstance, CardData cardData)
    {
        CardLogic cardLogic = cardInstance.GetComponent<CardLogic>();
        if (cardLogic == null)
        {
            card.AddComponent<CardLogic>();
        }
        cardLogic.SetCardData(cardData);
        cardLogic.SetUseImage();
        cardLogic.SetUseCount(GameManager.Instance.GetCardUseData(cardData.getCardName));

    }



    void CleanupOrphanedInstances()
    {
        // 找出需要清理的实例（卡牌数据中不存在的ID）
        List<string> keysToRemove = new List<string>();

        foreach (var kvp in createdCardInstances)
        {
            string cardId = kvp.Key;
            GameObject instance = kvp.Value;

            // 检查这个ID是否还在当前的cardData中
            bool stillExists = cardData.Exists(card => card.getCardName == cardId);

            if (!stillExists)
            {
                // 如果卡牌数据中不存在这个ID，标记为需要清理
                keysToRemove.Add(cardId);
                if (instance != null)
                {
                    Destroy(instance);
                    Debug.Log($"清理孤立的卡牌实例: {cardId}");
                }
            }
        }

        // 从字典中移除
        foreach (string key in keysToRemove)
        {
            createdCardInstances.Remove(key);
        }
    }
}
