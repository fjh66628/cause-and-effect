using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CardData
{
    [Header("卡牌内容")]
    [SerializeField] private PlayerState playerState;//卡牌内容
    public PlayerState getPlayerState => playerState;//获取玩家状态
    [Header("卡牌图片")]
    [SerializeField] private Sprite cardSprite;//卡牌图片
    public Sprite getCardSprite => cardSprite;//获取卡牌图片
    [Header("卡牌描述")]
    [SerializeField] private string cardDescription;//卡牌描述
    public string getCardDescription => cardDescription;//获取卡牌描述
    [Header("卡牌名称")]
    [SerializeField] private string cardName;//卡牌名称
    public string getCardName => cardName;//获取卡牌名称
}
