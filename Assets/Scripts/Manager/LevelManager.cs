using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]


public class UnitPosition//其他单位位置
{
    [Header("单位类型")]
    [SerializeField] private MapCellContent mapCellContent;//单位类型
    [Header("单位初始位置")]
    [SerializeField] private Vector2Int position;//单位初始位置
    [Header("单位ID(看注释)")]
    [SerializeField] private string id = "0";//单位ID
    /*门和钥匙（踏板）单位要用同一个id，否则会导致门和钥匙无法匹配。id的格式一共3位，3位分别对应
    门和钥匙的rgb值，使得系统能够生成符合视觉表现的门和钥匙。除开门和钥匙的id需要手动设置为0*/
    public MapCellContent getMapCellContent => mapCellContent;//获取单位类型
    public Vector2Int getPosition => position;//获取单位初始位置
    public string getId => id;//获取单位ID
}
[System.Serializable]
public class CardDataManagement//关卡的卡牌内容
{
    [Header("卡牌内容")]
    [SerializeField] private PlayerState cardType;//卡牌内容
    public PlayerState getCardType => cardType;//获取卡牌
    [Header("第几周目获得")]
    [SerializeField] private int round = 1;//第几回合获得
    [Header("当前层数可以使用几次")]
    [SerializeField] private int useCount = 1;//当前层数可以使用几次
    public int getRound => round;//获取第几回合获得
    public int getUseCount => useCount;//获取当前层数可以使用几次
}

[System.Serializable]
public class LevelManagement//储存关卡信息的数据结构
{
    [Header("玩家初始位置")]
    [SerializeField] private Vector2Int playerStartPosition;//玩家初始位置
    [Header("其他单位位置")]
    [SerializeField] private List<UnitPosition> unitPositions = new List<UnitPosition>();//其他单位位置
    [Header("地图大小")]
    [SerializeField] private Vector2Int mapSize;//地图大小
    [Header("需要走过几次终点")]
    [SerializeField] private int endStepCount = 1;//需要走过几次终点
    [Header("关卡的卡牌内容")]
    [SerializeField] private List<CardDataManagement> cardData = new List<CardDataManagement>();//关卡的卡牌内容
    [Header("是否为塌陷关卡")]
    [SerializeField] private bool isCollapse = false;//是否为塌陷关卡
    public bool getIsCollapse => isCollapse;//获取是否为塌陷关卡
    public int getEndStepCount => endStepCount;//获取需要走过几次终点
    public Vector2Int getMapSize => mapSize;//获取地图大小
    public List<UnitPosition> getUnitPositions => unitPositions;//获取其他单位位置
    public Vector2Int getPlayerStartPosition => playerStartPosition;//获取玩家初始位置
    public List<CardDataManagement> getCardData => cardData;//获取关卡的卡牌内容
}
[System.Serializable]
public class ChapterManagement//储存章节信息的数据结构
{
    [Header("关卡信息")]
    [SerializeField] private List<LevelManagement> levelManagements = new List<LevelManagement>();//关卡信息
    public List<LevelManagement> getLevelManagement => levelManagements;//获取关卡信息
}





[System.Serializable]
public class LevelManager : SingletonMono<LevelManager>
{
    [Header("传入卡牌信息")]
    [SerializeField] private CardDataSO cardData;//传入卡牌数据
    public List<CardData> getCardData => cardData.getCardList;//获取卡牌数据
    [Header("章节信息")]
    [SerializeField] private List<ChapterManagement> Chapter = new List<ChapterManagement>();//章节编号
    public int getChapterNumber => Chapter.Count;//获取章节数量
    public ChapterManagement getChapter => Chapter[GameManager.Instance.getChapterNumber - 1];//获取章节编号

    public LevelManagement GetLevelManagement(int chapterNumber, int levelNumber)//获取关卡信息
    {
        return Chapter[chapterNumber - 1].getLevelManagement[levelNumber - 1];
    }

    public List<CardData> GetCardData(int chapterNumber, int levelNumber, int endStepCount)//获取关卡的卡牌内容
    {
        List<CardData> cardDataList = new List<CardData>();//卡牌数据
        List<CardDataManagement> cardDataManagement = GetLevelManagement(chapterNumber, levelNumber).getCardData;//返回卡牌内容
        foreach (CardDataManagement card in cardDataManagement)
        {
            if (card.getRound <= endStepCount)
            {
                cardDataList.Add(getCardData.Find(x => x.getCardType == card.getCardType));//添加卡牌内容
            }
        }
        return cardDataList;
    }


    public Vector2Int getPlayerStartPosition => GetLevelManagement(GameManager.Instance.getChapterNumber, GameManager.Instance.getLevelNumber).getPlayerStartPosition;//获取玩家初始位置
}
