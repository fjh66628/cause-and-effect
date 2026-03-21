using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class GameManager : SingletonMono<GameManager>
{
    [Header("游戏状态")]
    [SerializeField] private GameState gameState = GameState.Start;//游戏状态
    [SerializeField] private PlayerState playerState = PlayerState.Stand;//玩家状态
    [SerializeField] private bool isPlayerMoving = false;//玩家是否正在移动
    [Header("走过了几次终点")]
    [SerializeField] private int endStepCount = 1;//走过了几次终点
    [Header("蓝条量")]
    [SerializeField] private int flyCount = 1;//飞行蓝条量
    [SerializeField] private int passWallCount = 1;//穿墙最大蓝条量
    [SerializeField] private int breakWallCount = 1;//破坏墙最大蓝条量
    [SerializeField] private int maxPassWallCount = 1;//穿墙最大蓝条量
    [SerializeField] private int maxBreakWallCount = 1;//破坏墙最大蓝条量
    [SerializeField] private int maxFlyCount = 1;//飞行最大蓝条量


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
    [SerializeField] private int ChapterNumber = 1;//章节编号
    [Header("关卡")]
    [SerializeField] private int levelNumber = 1;//关卡编号
    [Header("卡牌数据")]
    [SerializeField] private List<CardData> cardData;//传入卡牌数据

    public int getChapterNumber => ChapterNumber;//获取章节编号
    public int getLevelNumber => levelNumber;//获取关卡编号
    public GameState getGameState => gameState;//获取游戏状态

    void OnEnable()
    {
        EventHandler.playerUseSkill += UseSkill;
        EventHandler.updateCard += SetCardData;
        EventHandler.playerMove += OnPlayerMove;
    }
    void OnDisable()
    {
        EventHandler.playerUseSkill -= UseSkill;
        EventHandler.updateCard -= SetCardData;
        EventHandler.playerMove -= OnPlayerMove;
    }

    void Start()
    {
        EventHandler.CallLevelLoaded();
        EventHandler.CallUpdateCard();//调用更新卡牌数据事件
    }

    public void SetGameState(GameState state)//设置游戏状态
    {
        gameState = state;
    }
    public void SetCardData()//设置传入卡牌数据
    {
        this.cardData = LevelManager.Instance.GetCardData(ChapterNumber, levelNumber, endStepCount);//传入卡牌数据赋值
    }


    public bool ReachTheEnd()//玩家到达目标单元格
    {
        endStepCount++;//走过了几次终点增加
        if (endStepCount > LevelManager.Instance.GetLevelManagement(ChapterNumber, levelNumber).getEndStepCount)//如果走过了几次终点大于等于需要走过了几次终点
        {
            StartCoroutine(LoadNextLevel());//加载下一关
            EventHandler.CallUpdateCard();//调用更新卡牌数据事件
            return true;//返回true
        }
        else
        {
            EventHandler.CallUpdateCard();//调用更新卡牌数据事件
            return false;//返回false
        }
    }

    IEnumerator LoadNextLevel()//加载下一关
    {
        gameState = GameState.Pause;//游戏状态重置为暂停
        string currentSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";//获取当前场景名称
        levelNumber++;//关卡编号增加
        if (levelNumber > LevelManager.Instance.getChapter.getLevelManagement.Count)//如果关卡编号大于章节中的关卡数量
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

        yield return SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);//等待场景加载完成
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextSceneName));//设置下一关场景为活动场景
        endStepCount = 1;//重置走过了几次终点
        EventHandler.CallLevelLoaded();//调用关卡加载事件
        LoadSceneFadeIn();//加载场景淡入动画

        List<CardDataManagement> cardList = LevelManager.Instance.GetLevelManagement(ChapterNumber, levelNumber).getCardData;//获取所有卡片数据
        foreach (CardDataManagement card in cardList)//遍历所有卡片数据
        {
            switch (card.getCardType)
            {
                case PlayerState.Fly://飞行技能
                    maxFlyCount = card.getUseCount;//设置最大最大蓝条量
                    break;
                case PlayerState.PassWall://穿墙技能
                    maxPassWallCount = card.getUseCount;//设置最大最大蓝条量
                    break;
                case PlayerState.BreakWall://破坏墙技能
                    maxBreakWallCount = card.getUseCount;//设置最大最大蓝条量
                    break;
            }
        }//设置所有卡片类型
        flyCount = maxFlyCount;//设置飞行蓝条量为最大蓝条量
        passWallCount = maxPassWallCount;//设置穿墙最大蓝条量为最大蓝条量
        breakWallCount = maxBreakWallCount;//设置破坏墙最大蓝条量为蓝条量



        yield return new WaitForSeconds(0.5f);//等待0.5秒

        gameState = GameState.Play;//游戏状态重置为播放

    }

    public List<CardData> GetCardData()//获取传入卡牌数据
    {
        return cardData;//返回传入卡牌数据
    }

    void LoadSceneFadeOut()//加载场景动画
    {
        Debug.Log("淡出场景动画");
    }
    void LoadSceneFadeIn()//加载场景动画
    {
        Debug.Log("淡入场景动画");
    }

    public void OnPlayerMove()//玩家移动
    {
        switch (playerState)
        {
            case PlayerState.Fly://飞行技能
                flyCount--;//飞行蓝条量削减
                break;
            case PlayerState.PassWall://穿墙技能
                passWallCount--;//穿墙蓝条量削减
                break;
            case PlayerState.BreakWall://破坏墙技能
                breakWallCount--;//破坏墙蓝条量削减
                break;
        }
        if (flyCount <= 0 || passWallCount <= 0 || breakWallCount <= 0)//如果飞行蓝条量小于穿墙蓝条量小于破坏墙蓝条量小于等于0
        {
            playerState = PlayerState.Stand;//设置玩家状态为站立
            EventHandler.CallPlayerStand();//调用更新卡牌数据事件
            Debug.Log("玩家退出特殊状态");//输出玩家状态为站立日志
        }

    }

    public void UseSkill(CardData card)//使用技能 
    {
        if (playerState == PlayerState.Stand)//如果玩家状态为站立
        {
            switch (card.getCardType)
            {
                case PlayerState.Fly://飞行技能
                    playerState = PlayerState.Fly;//设置玩家状态为飞行
                    break;
                case PlayerState.PassWall://穿墙技能
                    playerState = PlayerState.PassWall;//设置玩家状态为穿墙
                    break;
                case PlayerState.BreakWall://破坏墙技能
                    playerState = PlayerState.BreakWall;//设置玩家状态为破坏墙
                    break;
            }
        }
        else
        {
            Debug.Log("玩家非站立状态不能使用技能");
        }
    }

    /// <summary>
    /// 重新加载当前关卡（重置游戏状态但保持同一关卡）
    /// </summary>
    public void ReloadCurrentLevel()
    {
        StartCoroutine(ReloadCurrentLevelCoroutine());
    }

    IEnumerator ReloadCurrentLevelCoroutine()
    {
        Debug.Log($"重新加载关卡: Chapter{ChapterNumber}Floor{levelNumber}");

        // 1. 暂停游戏
        gameState = GameState.Pause;

        // 2. 播放淡出动画
        LoadSceneFadeOut();
        yield return new WaitForSeconds(0.5f);

        // 3. 卸载当前场景
        string currentSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";
        yield return SceneManager.UnloadSceneAsync(currentSceneName);

        // 4. 清理资源
        Resources.UnloadUnusedAssets();
        yield return null;

        // 5. 重新加载同一场景
        yield return SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));

        // 6. 重置游戏状态
        ResetGameStateForReload();

        // 7. 播放淡入动画并恢复游戏
        LoadSceneFadeIn();
        gameState = GameState.Play;

        Debug.Log("关卡重新加载完成");
    }

    /// <summary>
    /// 重置游戏状态（重新加载时调用）
    /// </summary>
    private void ResetGameStateForReload()
    {
        // 重置步数计数
        endStepCount = 1;

        // 重置玩家状态
        playerState = PlayerState.Stand;
        isPlayerMoving = false;

        // 重置技能使用次数
        flyCount = maxFlyCount;
        passWallCount = maxPassWallCount;
        breakWallCount = maxBreakWallCount;

        // 重新获取卡牌数据
        SetCardData();

        // 触发关卡加载事件
        EventHandler.CallLevelLoaded();

        Debug.Log("游戏状态已重置");
    }

}
