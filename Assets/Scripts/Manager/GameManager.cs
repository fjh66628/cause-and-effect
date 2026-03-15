using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameState//游戏状态
{
    Start,//开始
    PlayerMove,//玩家移动
    EnemyMove,//敌人移动
    Play,//进行中
    Pause,//暂停
    End,//结束
}
[System.Serializable]
public class GameManager : SingletonMono<GameManager>
{
    [Header("游戏状态")]
    [SerializeField]private GameState gameState = GameState.Start;//游戏状态
    public GameState getGameState => gameState;//获取游戏状态
    public void SetGameState(GameState state)//设置游戏状态
    {
        gameState = state;
    }
}
