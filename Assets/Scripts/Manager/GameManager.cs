using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameManager : SingletonMono<GameManager>
{
    [Header("游戏状态")]
    [SerializeField]private GameState gameState = GameState.Start;//游戏状态
    [SerializeField]private PlayerState playerState = PlayerState.Stand;//玩家状态
    [SerializeField]private bool isPlayerMoving = false;//玩家是否正在移动
    [Header("走过了几次终点")]
    [SerializeField]private int endStepCount = 1;//走过了几次终点
    public int getEndStepCount => endStepCount;//获取走过了几次终点
    public bool IsPlayerMoving => isPlayerMoving;//是否玩家正在移动
    public void SetIsPlayerMoving(bool isMoving)//设置玩家是否正在移动
    {
        isPlayerMoving = isMoving;//玩家是否正在移动赋值
    }
    public void ChangePlayerState(PlayerState state)//改变玩家状态
    {
        playerState = state;//玩家状态赋值
    }
    public PlayerState getPlayerState => playerState;//获取玩家状态
    [Header("章节")]
    [SerializeField]private int ChapterNumber = 1;//章节编号
    [Header("关卡")]
    [SerializeField]private int levelNumber = 1;//关卡编号

    public int getChapterNumber => ChapterNumber;//获取章节编号
    public int getLevelNumber => levelNumber;//获取关卡编号
    public GameState getGameState => gameState;//获取游戏状态

    void Start()
    {
        EventHandler.CallLevelLoaded();
    }

    public void SetGameState(GameState state)//设置游戏状态
    {
        gameState = state;
    }
    public bool ReachTheEnd()//玩家到达目标单元格
    {
        endStepCount++;//走过了几次终点增加
        if(endStepCount > LevelManager.Instance.GetLevelManagement(ChapterNumber,levelNumber).getEndStepCount)//如果走过了几次终点大于等于需要走过了几次终点
        {
            StartCoroutine(LoadNextLevel());//加载下一关
            return true;//返回true
        }
        else
        {
            return false;//返回false
        }
    }

    IEnumerator LoadNextLevel()//加载下一关
    {
        gameState = GameState.Pause;//游戏状态重置为暂停
        string currentSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";//获取当前场景名称
        levelNumber++;//关卡编号增加
        if(levelNumber > LevelManager.Instance.getChapter.getLevelManagement.Count)//如果关卡编号大于章节中的关卡数量
        {
            levelNumber = 1;//关卡编号重置为1
            ChapterNumber++;//章节编号增加
        }
        string nextSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";//下一关场景名称

        LoadSceneFadeOut();//加载场景淡出动画;
        yield return new WaitForSeconds(0.5f);//等待0.5秒

        yield return SceneManager.UnloadSceneAsync(currentSceneName);

        // 1. 卸载无用资源
        Resources.UnloadUnusedAssets();
        yield return null;

        yield return SceneManager.LoadSceneAsync(nextSceneName,LoadSceneMode.Additive);//等待场景加载完成
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextSceneName));//设置下一关场景为活动场景
        endStepCount=1;//重置走过了几次终点
        EventHandler.CallLevelLoaded();//调用关卡加载事件
        LoadSceneFadeIn();//加载场景淡入动画
        yield return new WaitForSeconds(0.5f);//等待0.5秒
        gameState = GameState.Play;//游戏状态重置为播放
    }

    void LoadSceneFadeOut()//加载场景动画
    {
        Debug.Log("淡出场景动画");
    }
    void LoadSceneFadeIn()//加载场景动画
    {
        Debug.Log("淡入场景动画");
    }
}
