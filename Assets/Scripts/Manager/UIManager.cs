using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<CardData> cardData;//卡牌数据
    void OnEnable()
    {
        EventHandler.updateCard += SetCardData;
    }
    void OnDisable()
    {
        EventHandler.updateCard -= SetCardData;
    }
    [Header("卡牌")]
    [SerializeField] private GameObject card;//卡牌图片

    void Start()
    {
        SetCardData();//设置传牌数据
    }

    public void SetCardData()//设置传入卡牌数据
    {
        StartCoroutine(UpdateCardData());//更新传牌数据

    }
    IEnumerator UpdateCardData()//更新传牌数据
    {
        yield return null;
        this.cardData = GameManager.Instance.GetCardData();//传入卡牌数据赋值
        Debug.Log(this.cardData[0].getCardName);
        GameObject targetSceneRoot = GameObject.Find($"卡牌容器"); // 找到目标场景的根节点
        foreach (var item in cardData)
        {
            GameObject cardImage = CreateCardImage(item);
            cardImage.transform.SetParent(targetSceneRoot.GetComponent<RectTransform>());
        }
    }
    GameObject CreateCardImage(CardData cardData)//创建卡牌按钮
    {
        GameObject cardImage = Instantiate(card);
        CardLogic cardLogic = cardImage.GetComponent<CardLogic>();
        Debug.Log(cardLogic);
        if (cardLogic == null)
        {
            cardLogic = cardImage.AddComponent<CardLogic>();
        }
        cardLogic.SetCardData(cardData);
        return cardImage;
    }
}
