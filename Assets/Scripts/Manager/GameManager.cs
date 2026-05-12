using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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
    [SerializeField] private int passWallCell = 1;//穿墙可以穿过几个墙单元格
    [SerializeField] private int flyCell = 1;//飞行可以飞行几个墙单元格

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
    bool toWall = false;//是否玩家到达墙单元格
    bool toWater = false;//是否玩家到达水单元格
    bool toCollapse = false;//是否玩家到达塌陷处单元格
    bool toDoor_locked = false;//是否玩家到达门单元格
    bool toDoor_opened = false;//是否玩家到达门单元格
    bool toKey = false;//是否玩家到达钥匙单元格
    bool toDoor_singleuse = false;//是否玩家到达一次性门单元格
    bool toWall_unbreakable = false;//是否玩家到达不可破坏的墙单元格
    [SerializeField] private DialogueSO toTheWall;//对话
    [SerializeField] private DialogueSO toTheWater;//对话
    [SerializeField] private DialogueSO toTheCollapse;//对话
    [SerializeField] private DialogueSO toTheDoor_locked;//对话
    [SerializeField] private DialogueSO toTheDoor_opened;//对话
    [SerializeField] private DialogueSO toTheKey;//对话
    [SerializeField] private DialogueSO toTheDoor_singleuse;//对话
    [SerializeField] private DialogueSO toTheWall_unbreakable;//对话


    public int getChapterNumber => ChapterNumber;//获取章节编号
    public int getLevelNumber => levelNumber;//获取关卡编号
    public GameState getGameState => gameState;//获取游戏状态

    void OnEnable()
    {
        EventHandler.playerUseSkill += UseSkill;
        EventHandler.updateCard += SetCardData;
        EventHandler.playerMove += OnPlayerMove;
        EventHandler.levelLoaded += SetCardNumber;
    }
    void OnDisable()
    {
        EventHandler.playerUseSkill -= UseSkill;
        EventHandler.updateCard -= SetCardData;
        EventHandler.playerMove -= OnPlayerMove;
        EventHandler.levelLoaded -= SetCardNumber;
    }



    public void SetGameState(GameState state)//设置游戏状态
    {
        gameState = state;
    }
    public void SetCardData()//设置传入卡牌数据
    {
        List<CardData> removeCardList = new List<CardData>();//移除卡片列表
        this.cardData = LevelManager.Instance.GetCardData(ChapterNumber, levelNumber, endStepCount);//传入卡牌数据赋值
        foreach (CardData card in cardData)//遍历卡牌数据
        {
            if (card.getCardType == PlayerState.Fly && flyCount <= 0 || card.getCardType == PlayerState.PassWall && passWallCount <= 0 || card.getCardType == PlayerState.BreakWall && breakWallCount <= 0)//如果飞行蓝条量小于等于0或穿墙蓝条量小于等于0或破坏墙蓝条量小于等于0
            {
                removeCardList.Add(card);//移除卡片列表
            }
        }
        foreach (CardData card in removeCardList)//遍历移除卡片列表
        {
            RemoveCardFromData(card);//移除卡片
        }
    }


    public bool ReachTheEnd()//玩家到达目标单元格
    {
        endStepCount++;//走过了几次终点增加
        if (endStepCount > LevelManager.Instance.GetLevelManagement(ChapterNumber, levelNumber).getEndStepCount)//如果走过了几次终点大于等于需要走过了几次终点
        {
            string currentSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";//获取当前场景名称
            levelNumber++;//关卡编号增加
            if (levelNumber > LevelManager.Instance.getChapter.getLevelManagement.Count)//如果关卡编号大于章节中的关卡数量
            {
                levelNumber = 1;//关卡编号重置为1
                ChapterNumber++;//章节编号增加
            }
            string nextSceneName = $"Chapter{ChapterNumber}Floor{levelNumber}";//下一关场景名称

            if (LevelManager.Instance.getChapterNumber < ChapterNumber || LevelManager.Instance.getChapter.getLevelManagement.Count < levelNumber)//如果章节中的关卡数量等于关卡编号
            {
                LoadingAnimator.Instance.SetLoading("游戏结束");
                StartCoroutine(LoadEndScene());//加载结束场景
                return true;//返回true
            }
            LoadingAnimator.Instance.SetLoading(nextSceneName);//加载下一关
            StartCoroutine(LoadNextLevel(currentSceneName, nextSceneName));//加载下一关
            EventHandler.CallUpdateCard();//调用更新卡牌数据事件
            return true;//返回true
        }
        else
        {
            LoadingAnimator.Instance.SetLoading("史莱姆又踏上了轮回...");
            EventHandler.CallUpdateCard();//调用更新卡牌数据事件
            return false;//返回false
        }
    }

    IEnumerator LoadNextLevel(string currentSceneName, string nextSceneName)//加载下一关
    {
        gameState = GameState.Pause;//游戏状态重置为暂停
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

        SetCardNumber();//设置卡牌蓝数目

        EventHandler.CallUpdateCard();//调用更新卡牌数据事件


        yield return new WaitForSeconds(2f);//等待2秒

        gameState = GameState.Play;//游戏状态重置为播放

    }
    void SetCardNumber()//设置卡牌蓝数目
    {
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
                flyCell--;//设置飞行削减
                break;
            case PlayerState.PassWall://穿墙技能
                passWallCell--;//设置穿墙削减
                break;
        }
        if (flyCell == -1 || passWallCell == -1)//如果飞行蓝或穿墙小于0
        {
            playerState = PlayerState.Stand;//设置玩家状态为站立        
            EventHandler.CallPlayerStand();//调用更新玩家状态事件
            if (flyCell == -1)//如果飞行蓝小于0
            {
                flyCell = -2;
            }
            if (passWallCell == -1)//如果穿墙小于0` 
            {
                passWallCell = -2;
            }
            MapManager mapManager = FindObjectOfType<MapManager>();//更新地图状态
            mapManager.CheckPlayerPosition();//更新地图状态
        }

    }

    public void UseSkill(CardData card)//使用技能 
    {
        if (playerState != PlayerState.Stand)
        {
            Debug.Log("重复使用卡牌会刷新状态，也会覆盖掉之前的状态");
        }
        if (card.getCardType == PlayerState.Fly && flyCount > 0)//飞行技能且蓝条量大于0
        {
            playerState = PlayerState.Fly;//设置玩家状态为飞行
            flyCount--;//飞行蓝条量削减
            flyCell = 1;//设置飞行蓝条量为穿墙单元格
        }
        else if (card.getCardType == PlayerState.PassWall && passWallCount > 0)//穿墙技能且蓝条量大于0
        {
            playerState = PlayerState.PassWall;//设置玩家状态为穿墙
            passWallCount--;//穿墙蓝条量削减
            passWallCell = 1;//设置穿墙蓝条量为穿墙单元格

        }
        else if (card.getCardType == PlayerState.BreakWall && breakWallCount > 0)//破坏墙技能且蓝条量大于0
        {
            playerState = PlayerState.BreakWall;//设置玩家状态为破坏墙
            breakWallCount--;//破坏墙蓝条量削减
        }
        else
        {
            // 如果蓝条量不足，给出提示
            string skillName = card.getCardName;
            Debug.LogWarning($"无法使用{skillName}技能，蓝条量不足");
        }
        EventHandler.CallUpdateCard();//调用更新卡牌数据事件
    }
    void RemoveCardFromData(CardData cardToRemove)
    {
        if (cardData == null || cardToRemove == null)
        {
            Debug.LogWarning("卡牌数据为空或要移除的卡牌为空");
            return;
        }

        // 查找并移除卡牌
        int removedCount = cardData.RemoveAll(card => card == cardToRemove);

        if (removedCount > 0)
        {
            Debug.Log($"成功移除卡牌: {cardToRemove.getCardName}，移除数量: {removedCount}");
        }
        else
        {
            Debug.LogWarning($"未找到要移除的卡牌: {cardToRemove.getCardName}");
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
        EventHandler.CallUpdateCard();//调用更新卡牌数据事件
        Debug.Log("游戏状态已重置");
    }

    public int GetCardUseData(string cardName)//获取卡牌使用次数
    {
        foreach (var item in cardData)
        {
            if (item.getCardName == cardName)
            {
                switch (item.getCardType)
                {
                    case PlayerState.Fly://飞行技能
                        return flyCount;
                    case PlayerState.PassWall://穿墙技能
                        return passWallCount;
                    case PlayerState.BreakWall://破坏墙技能
                        return breakWallCount;
                }
            }
        }
        return 0;
    }

    IEnumerator LoadEndScene()
    {
        gameState = GameState.Pause;//游戏状态重置为暂停

        // 播放淡出动画
        LoadSceneFadeOut();
        yield return new WaitForSeconds(0.5f);//等待0.5秒


        // 加载结束场景（单场景模式）
        yield return SceneManager.LoadSceneAsync("END", LoadSceneMode.Single);
        LoadSceneFadeIn();//播放淡入动画
        yield return new WaitForSeconds(0.5f);//等待0.5秒

    }

    public string GetBuffText()
    {
        switch (playerState)
        {
            case PlayerState.Fly://飞行技能
                return "可以飞过1个格子";
            case PlayerState.PassWall://穿墙技能
                return "可以穿过1个墙格子";
            case PlayerState.BreakWall://破坏墙技能
                return "可以破坏1个墙格子";
        }
        return "当前没有特殊状态";
    }

    public void SetLevelCount(int Chapter, int Level)
    {
        ChapterNumber = Chapter;
        levelNumber = Level;
    }

    public void GiveTips(MapCellContent mapCellContent)
    {
        switch (mapCellContent)
        {
            case MapCellContent.Wall:
                if (!toWall)
                {
                    toWall = true;
                    EventHandler.showDialogue(toTheWall);
                }
                break;
            case MapCellContent.Water:
                if (!toWater)
                {
                    toWater = true;
                    EventHandler.showDialogue(toTheWater);
                }
                break;
            case MapCellContent.Collapse:
                if (!toCollapse)
                {
                    toCollapse = true;
                    EventHandler.showDialogue(toTheCollapse);
                }
                break;
            case MapCellContent.Wall_unbreakable:
                if (!toWall_unbreakable)
                {
                    toWall_unbreakable = true;
                    EventHandler.showDialogue(toTheWall_unbreakable);
                }
                break;
            case MapCellContent.Door_locked:
                if (!toDoor_locked)
                {
                    toDoor_locked = true;
                    EventHandler.showDialogue(toTheDoor_locked);
                }
                break;
            case MapCellContent.Door_opened:
                if (!toDoor_opened)
                {
                    toDoor_opened = true;
                    EventHandler.showDialogue(toTheDoor_opened);
                }
                break;
            case MapCellContent.Key:
                if (!toKey)
                {
                    toKey = true;
                    EventHandler.showDialogue(toTheKey);
                }
                break;
            case MapCellContent.Door_singleuse:
                if (!toDoor_singleuse)
                {
                    toDoor_singleuse = true;
                    EventHandler.showDialogue(toTheDoor_singleuse);
                }
                break;
        }
    }
}
