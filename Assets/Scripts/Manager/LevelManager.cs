using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonMono<LevelManager>
{
    [Header("关卡编号")]
    [SerializeField]private int levelNumber = 1;//关卡编号
    public int getLevelNumber => levelNumber;//获取关卡编号
    [Header("玩家初始位置")]
    [SerializeField]private List<Vector2Int> playerStartPosition = new List<Vector2Int>() { new Vector2Int(0, 0) };//玩家初始位置




    public Vector2Int getPlayerStartPosition => playerStartPosition[LevelManager.Instance.getLevelNumber-1];//获取玩家初始位置
}
