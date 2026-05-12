using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class EventHandler
{
    public static Action<PlayerState> playerStateChange;//玩家状态改变事件
    public static Action<Vector2> onMouseLeftClick;//鼠标点击左键事件
    public static Action levelLoaded;//关卡加载事件
    public static Action breakTheWall;//玩家破坏墙单元格事件
    public static Action mapLoaded;//地图加载事件
    public static Action<MapCellContent, Vector2Int> changeItem;//改变物品事件
    public static Action<Vector2Int> passTheWall;//玩家穿墙单元格事件

    public static Action<CardData> playerUseSkill;//玩家使用技能事件
    public static Action updateCard;//更新卡牌数据事件
    public static Action playerMove;//改变玩家位置事件
    public static Action playerStand;//玩家退出特殊状态事件
    public static Action<DialogueSO> showDialogue;//显示对话框事件
    public static Action<string> gameOver;//游戏失败事件
    public static Action onMouseClick;//鼠标点击事件


    public static void CallMouseLeftClick(Vector2 position)//调用鼠标点击左键事件
    {
        onMouseLeftClick?.Invoke(position);
    }
    public static void CallLevelLoaded()//调用关卡加载事件
    {
        levelLoaded?.Invoke();
    }
    public static void CallBreakTheWall()//调用玩家破坏墙单元格事件
    {
        breakTheWall?.Invoke();
    }
    public static void CallMapLoaded()//调用地图加载事件
    {
        mapLoaded?.Invoke();
    }
    public static void CallChangeItem(MapCellContent mapCellContent, Vector2Int position)//调用改变物品事件
    {
        changeItem?.Invoke(mapCellContent, position);
    }
    public static void CallPassTheWall(Vector2Int position)//调用玩家穿墙单元格事件
    {
        passTheWall?.Invoke(position);
    }

    public static void CallPlayerUseSkill(CardData card)//调用玩家使用技能事件
    {
        playerUseSkill?.Invoke(card);
    }
    public static void CallUpdateCard()//调用更新卡牌数据事件
    {
        updateCard?.Invoke();
    }
    public static void CallPlayerMove()//调用改变玩家位置事件
    {
        playerMove?.Invoke();
    }
    public static void CallPlayerStand()//调用玩家退出特殊状态事件
    {
        playerStand?.Invoke();
    }
    public static void CallShowDialogue(DialogueSO dialogueData)//调用显示对话框事件
    {
        showDialogue?.Invoke(dialogueData);
    }
    public static void CallGameOver(string reason)//调用游戏失败事件
    {
        gameOver?.Invoke(reason);
    }
    public static void CallMouseClick()//调用鼠标点击事件
    {
        onMouseClick?.Invoke();
    }
    public static void CallPlayerStateChange(PlayerState state)//调用更新玩家状态事件
    {
        playerStateChange?.Invoke(state);
    }
}
