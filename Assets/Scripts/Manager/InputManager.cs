using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMono<InputManager>
{

    void Update()
    {
        if(GameManager.Instance.getGameState == GameState.PlayerMove)//如果游戏状态是玩家移动
        {
            if (Input.GetMouseButtonDown(0))//如果点击了鼠标左键
            {
                EventHandler.CallMouseLeftClick(CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition));//调用鼠标点击左键事件
            }
        }
    }
}
