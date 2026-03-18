using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]


public class UnitPosition//其他单位位置
{
    [Header("单位类型")]
    [SerializeField]private MapCellContent mapCellContent;//单位类型
    [Header("单位初始位置")]
    [SerializeField]private Vector2Int position;//单位初始位置
    [Header("单位ID(看注释)")]
    [SerializeField]private string id="0";//单位ID
    /*门和钥匙（踏板）单位要用同一个id，否则会导致门和钥匙无法匹配。id的格式一共3位，3位分别对应
    门和钥匙的rgb值，使得系统能够生成符合视觉表现的门和钥匙。除开门和钥匙的id需要手动设置为0*/
    public MapCellContent getMapCellContent => mapCellContent;//获取单位类型
    public Vector2Int getPosition => position;//获取单位初始位置
    public string getId => id;//获取单位ID
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
    public int getEndStepCount => endStepCount;//获取需要走过几次终点
    public Vector2Int getMapSize => mapSize;//获取地图大小
    public List<UnitPosition> getUnitPositions => unitPositions;//获取其他单位位置
    public Vector2Int getPlayerStartPosition => playerStartPosition;//获取玩家初始位置
}
[System.Serializable]
public class ChapterManagement//储存章节信息的数据结构
{
    [Header("关卡信息")]
    [SerializeField]private List<LevelManagement> levelManagements = new List<LevelManagement>();//关卡信息
    public List<LevelManagement> getLevelManagement => levelManagements;//获取关卡信息
}
[System.Serializable]
public class LevelManager : SingletonMono<LevelManager>
{
    [Header("章节信息")]
    [SerializeField]private List<ChapterManagement> Chapter = new List<ChapterManagement>();//章节编号
    public ChapterManagement getChapter => Chapter[GameManager.Instance.getChapterNumber-1];//获取章节编号

    public LevelManagement GetLevelManagement(int chapterNumber , int levelNumber)//获取关卡信息
    {
        return Chapter[chapterNumber-1].getLevelManagement[levelNumber-1];
    }

    public Vector2Int getPlayerStartPosition => GetLevelManagement(GameManager.Instance.getChapterNumber,GameManager.Instance.getLevelNumber).getPlayerStartPosition;//获取玩家初始位置
}
