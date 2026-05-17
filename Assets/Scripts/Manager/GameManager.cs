using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
[System.Serializable]
public class GameManager : SingletonMono<GameManager>
{
    [Header("Game State")]
    [SerializeField] private GameState gameState = GameState.Start;//娓告垙鐘舵€?
    [SerializeField] private PlayerState playerState = PlayerState.Stand;//鐜╁鐘舵€?
    [SerializeField] private bool isPlayerMoving = false;//鐜╁鏄惁姝ｅ湪绉诲姩
    [Header("End Step Count")]
    [SerializeField] private int endStepCount = 1;//璧拌繃浜嗗嚑娆＄粓鐐?
    [Header("Skill Counts")]
    [SerializeField] private int flyCount = 1;//椋炶钃濇潯閲?
    [SerializeField] private int passWallCount = 1;//绌垮鏈€澶ц摑鏉￠噺
    [SerializeField] private int breakWallCount = 1;//鐮村潖澧欐渶澶ц摑鏉￠噺
    [SerializeField] private int maxPassWallCount = 1;//绌垮鏈€澶ц摑鏉￠噺
    [SerializeField] private int maxBreakWallCount = 1;//鐮村潖澧欐渶澶ц摑鏉￠噺
    [SerializeField] private int maxFlyCount = 1;//椋炶鏈€澶ц摑鏉￠噺
    [SerializeField] private int passWallCell = 1;//绌垮鍙互绌胯繃鍑犱釜澧欏崟鍏冩牸
    [SerializeField] private int flyCell = 1;//椋炶鍙互椋炶鍑犱釜澧欏崟鍏冩牸
    [SerializeField] private DialogueSO cardTips;//鍗＄墝鎻愮ず瀵硅瘽妗?鍦ㄩ潪绔欑珛鐘舵€佷笅鏄剧ず)
    public int getEndStepCount => endStepCount;//鑾峰彇璧拌繃浜嗗嚑娆＄粓鐐?
    public bool IsPlayerMoving => isPlayerMoving;//鏄惁鐜╁姝ｅ湪绉诲姩
    public void SetIsPlayerMoving(bool isMoving)//璁剧疆鐜╁鏄惁姝ｅ湪绉诲姩
    {
        isPlayerMoving = isMoving;//鐜╁鏄惁姝ｅ湪绉诲姩璧嬪€?
    }
    public void ChangePlayerState(PlayerState state)//鏀瑰彉鐜╁鐘舵€?
    {
        playerState = state;//鐜╁鐘舵€佽祴鍊?
    }
    public PlayerState getPlayerState => playerState;//鑾峰彇鐜╁鐘舵€?
    [Header("绔犺妭")]
    [SerializeField] private int ChapterNumber = 1;//绔犺妭缂栧彿
    [Header("鍏冲崱")]
    [SerializeField] private int levelNumber = 1;//鍏冲崱缂栧彿
    [Header("鍗＄墝鏁版嵁")]
    [SerializeField] private List<CardData> cardData;//浼犲叆鍗＄墝鏁版嵁
    bool toWall = false;//鏄惁鐜╁鍒拌揪澧欏崟鍏冩牸
    bool toWater = false;//鏄惁鐜╁鍒拌揪姘村崟鍏冩牸
    bool toCollapse = false;//鏄惁鐜╁鍒拌揪濉岄櫡澶勫崟鍏冩牸
    bool toDoor_locked = false;//鏄惁鐜╁鍒拌揪闂ㄥ崟鍏冩牸
    bool toDoor_opened = false;//鏄惁鐜╁鍒拌揪闂ㄥ崟鍏冩牸
    bool toKey = false;//鏄惁鐜╁鍒拌揪閽ュ寵鍗曞厓鏍?
    bool toDoor_singleuse = false;//鏄惁鐜╁鍒拌揪涓€娆℃€ч棬鍗曞厓鏍?
    bool toWall_unbreakable = false;//鏄惁鐜╁鍒拌揪涓嶅彲鐮村潖鐨勫鍗曞厓鏍?
    [SerializeField] private DialogueSO toTheWall;//瀵硅瘽
    [SerializeField] private DialogueSO toTheWater;//瀵硅瘽
    [SerializeField] private DialogueSO toTheCollapse;//瀵硅瘽
    [SerializeField] private DialogueSO toTheDoor_locked;//瀵硅瘽
    [SerializeField] private DialogueSO toTheDoor_opened;//瀵硅瘽
    [SerializeField] private DialogueSO toTheKey;//瀵硅瘽
    [SerializeField] private DialogueSO toTheDoor_singleuse;//瀵硅瘽
    [SerializeField] private DialogueSO toTheWall_unbreakable;//瀵硅瘽


    public int getChapterNumber => ChapterNumber;//鑾峰彇绔犺妭缂栧彿
    public int getLevelNumber => levelNumber;//鑾峰彇鍏冲崱缂栧彿
    public GameState getGameState => gameState;//鑾峰彇娓告垙鐘舵€?

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



    public void SetGameState(GameState state)//璁剧疆娓告垙鐘舵€?
    {
        gameState = state;
    }
    public void SetCardData()//璁剧疆浼犲叆鍗＄墝鏁版嵁
    {
        List<CardData> removeCardList = new List<CardData>();//绉婚櫎鍗＄墖鍒楄〃
        cardData = LevelManager.Instance.GetCardData(ChapterNumber, levelNumber, endStepCount);//浼犲叆鍗＄墝鏁版嵁璧嬪€?
        foreach (CardData card in cardData)//閬嶅巻鍗＄墝鏁版嵁
        {
            if (card.getCardType == PlayerState.Fly && flyCount <= 0 || card.getCardType == PlayerState.PassWall && passWallCount <= 0 || card.getCardType == PlayerState.BreakWall && breakWallCount <= 0)//濡傛灉椋炶钃濇潯閲忓皬浜庣瓑浜?鎴栫┛澧欒摑鏉￠噺灏忎簬绛変簬0鎴栫牬鍧忓钃濇潯閲忓皬浜庣瓑浜?
            {
                removeCardList.Add(card);//绉婚櫎鍗＄墖鍒楄〃
            }
        }
        foreach (CardData card in removeCardList)//閬嶅巻绉婚櫎鍗＄墖鍒楄〃
        {
            RemoveCardFromData(card);//绉婚櫎鍗＄墖
        }
    }


    public bool ReachTheEnd()
    {
        endStepCount++;
        if (endStepCount > LevelManager.Instance.GetLevelManagement(ChapterNumber, levelNumber).getEndStepCount)
        {
            LoadManager.Instance.LoadNextLevel();
            EventHandler.CallUpdateCard();
            return true;
        }

        LoadingAnimator.Instance.SetLoading("\u53f2\u83b1\u59c6\u53c8\u8e0f\u4e0a\u4e86\u8f6e\u56de...");
        EventHandler.CallUpdateCard();
        return false;
    }
    void SetCardNumber()//璁剧疆鍗＄墝钃濇暟鐩?
    {
        List<CardDataManagement> cardList = LevelManager.Instance.GetLevelManagement(ChapterNumber, levelNumber).getCardData;//鑾峰彇鎵€鏈夊崱鐗囨暟鎹?
        foreach (CardDataManagement card in cardList)//閬嶅巻鎵€鏈夊崱鐗囨暟鎹?
        {
            switch (card.getCardType)
            {
                case PlayerState.Fly://椋炶鎶€鑳?
                    maxFlyCount = card.getUseCount;//璁剧疆鏈€澶ф渶澶ц摑鏉￠噺
                    break;
                case PlayerState.PassWall://绌垮鎶€鑳?
                    maxPassWallCount = card.getUseCount;//璁剧疆鏈€澶ф渶澶ц摑鏉￠噺
                    break;
                case PlayerState.BreakWall://鐮村潖澧欐妧鑳?
                    maxBreakWallCount = card.getUseCount;//璁剧疆鏈€澶ф渶澶ц摑鏉￠噺
                    break;
            }
        }//璁剧疆鎵€鏈夊崱鐗囩被鍨?
        flyCount = maxFlyCount;//璁剧疆椋炶钃濇潯閲忎负鏈€澶ц摑鏉￠噺
        passWallCount = maxPassWallCount;//璁剧疆绌垮鏈€澶ц摑鏉￠噺涓烘渶澶ц摑鏉￠噺
        breakWallCount = maxBreakWallCount;//璁剧疆鐮村潖澧欐渶澶ц摑鏉￠噺涓鸿摑鏉￠噺
    }

    public List<CardData> GetCardData()//鑾峰彇浼犲叆鍗＄墝鏁版嵁
    {
        return cardData;//杩斿洖浼犲叆鍗＄墝鏁版嵁
    }


    public void OnPlayerMove()//鐜╁绉诲姩
    {
        switch (playerState)
        {
            case PlayerState.Fly://椋炶鎶€鑳?
                flyCell--;//璁剧疆椋炶鍓婂噺
                break;
            case PlayerState.PassWall://绌垮鎶€鑳?
                passWallCell--;//璁剧疆绌垮鍓婂噺
                break;
        }
        if (flyCell == -1 || passWallCell == -1)//濡傛灉椋炶钃濇垨绌垮灏忎簬0
        {
            playerState = PlayerState.Stand;//璁剧疆鐜╁鐘舵€佷负绔欑珛        
            if (flyCell == -1)//濡傛灉椋炶钃濆皬浜?
            {
                EventHandler.CallPlayerStand();//璋冪敤鏇存柊鐜╁鐘舵€佷簨浠?
                flyCell = -2;
            }
            if (passWallCell == -1)//濡傛灉绌垮灏忎簬0` 
            {
                passWallCell = -2;
            }
            MapManager mapManager = FindObjectOfType<MapManager>();//鏇存柊鍦板浘鐘舵€?
            mapManager.CheckPlayerPosition();//鏇存柊鍦板浘鐘舵€?
        }

    }

    public void UseSkill(CardData card)//浣跨敤鎶€鑳?
    {
        if (playerState != PlayerState.Stand)
        {
            gameState = GameState.Pause;//娓告垙鐘舵€侀噸缃负鏆傚仠
            FindObjectOfType<UIManager>().HaveDialogue(cardTips);//璋冪敤鏄剧ず瀵硅瘽妗嗕簨浠?
        }
        if (playerState == PlayerState.Stand)
            EventHandler.CallPlayerStateChange(card.getCardType);//璋冪敤鏇存柊鐜╁鐘舵€佷簨浠?
        if (card.getCardType == PlayerState.Fly && flyCount > 0)//椋炶鎶€鑳戒笖钃濇潯閲忓ぇ浜?
        {
            playerState = PlayerState.Fly;//璁剧疆鐜╁鐘舵€佷负椋炶
            flyCount--;//椋炶钃濇潯閲忓墛鍑?
            flyCell = 1;//璁剧疆椋炶钃濇潯閲忎负绌垮鍗曞厓鏍?
        }
        else if (card.getCardType == PlayerState.PassWall && passWallCount > 0)//绌垮鎶€鑳戒笖钃濇潯閲忓ぇ浜?
        {
            playerState = PlayerState.PassWall;//璁剧疆鐜╁鐘舵€佷负绌垮
            passWallCount--;//绌垮钃濇潯閲忓墛鍑?
            passWallCell = 1;//璁剧疆绌垮钃濇潯閲忎负绌垮鍗曞厓鏍?

        }
        else if (card.getCardType == PlayerState.BreakWall && breakWallCount > 0)//鐮村潖澧欐妧鑳戒笖钃濇潯閲忓ぇ浜?
        {
            playerState = PlayerState.BreakWall;//璁剧疆鐜╁鐘舵€佷负鐮村潖澧?
            breakWallCount--;//鐮村潖澧欒摑鏉￠噺鍓婂噺
        }
        else
        {
            // 濡傛灉钃濇潯閲忎笉瓒筹紝缁欏嚭鎻愮ず
            string skillName = card.getCardName;
            Debug.LogWarning($"Cannot use {skillName}, not enough count.");
        }
        EventHandler.CallUpdateCard();//璋冪敤鏇存柊鍗＄墝鏁版嵁浜嬩欢
    }
    void RemoveCardFromData(CardData cardToRemove)
    {
        if (cardData == null || cardToRemove == null)
        {
            Debug.LogWarning("Card data is null or the card to remove is null.");
            return;
        }

        // 鏌ユ壘骞剁Щ闄ゅ崱鐗?
        int removedCount = cardData.RemoveAll(card => card == cardToRemove);

    }
    public void ReloadCurrentLevel()
    {
        LoadManager.Instance.ReloadCurrentLevel();
    }

    public void ResetGameStateForLevelLoad()
    {
        endStepCount = 1;
        playerState = PlayerState.Stand;
        isPlayerMoving = false;
        flyCell = 1;
        passWallCell = 1;

        SetCardNumber();
        SetCardData();
    }
    public int GetCardUseData(string cardName)//鑾峰彇鍗＄墝浣跨敤娆℃暟
    {
        foreach (var item in cardData)
        {
            if (item.getCardName == cardName)
            {
                switch (item.getCardType)
                {
                    case PlayerState.Fly://椋炶鎶€鑳?
                        return flyCount;
                    case PlayerState.PassWall://绌垮鎶€鑳?
                        return passWallCount;
                    case PlayerState.BreakWall://鐮村潖澧欐妧鑳?
                        return breakWallCount;
                }
            }
        }
        return 0;
    }

    public string GetBuffText()
    {
        switch (playerState)
        {
            case PlayerState.Fly://椋炶鎶€鑳?
                return "\u53ef\u4ee5\u98de\u8fc71\u4e2a\u683c\u5b50";
            case PlayerState.PassWall://绌垮鎶€鑳?
                return "鍙互绌胯繃1涓鏍煎瓙";
            case PlayerState.BreakWall://鐮村潖澧欐妧鑳?
                return "鍙互鐮村潖1涓鏍煎瓙";
        }
        return "\u5f53\u524d\u6ca1\u6709\u7279\u6b8a\u72b6\u6001";
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


