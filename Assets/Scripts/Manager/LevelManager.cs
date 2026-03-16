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
    public MapCellContent getMapCellContent => mapCellContent;//获取单位类型
    public Vector2Int getPosition => position;//获取单位初始位置
}

[System.Serializable]
public class LevelManagement//储存关卡信息的数据结构
{
    [Header("玩家初始位置")]
    [SerializeField] private Vector2Int playerStartPosition;//玩家初始位置
    [Header("其他单位位置")]
    [SerializeField] private List<UnitPosition> unitPositions = new List<UnitPosition>();//其他单位位置
    public List<UnitPosition> getUnitPositions => unitPositions;//获取其他单位位置
    public Vector2Int getPlayerStartPosition => playerStartPosition;//获取玩家初始位置
}
public class LevelManager : SingletonMono<LevelManager>
{
    [Header("关卡编号")]
    [SerializeField]private int levelNumber = 1;//关卡编号
    public int getLevelNumber => levelNumber;//获取关卡编号
    [Header("关卡信息")]
    [SerializeField]private List<LevelManagement> levelManagements = new List<LevelManagement>();//关卡信息

    public LevelManagement getLevelManagement => levelManagements[levelNumber-1];//获取关卡信息

    public Vector2Int getPlayerStartPosition => levelManagements[LevelManager.Instance.getLevelNumber-1].getPlayerStartPosition;//获取玩家初始位置
}
