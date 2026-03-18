using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InputManager : SingletonMono<InputManager>
{

    void Update()
    {
        if(GameManager.Instance.getGameState == GameState.Play && !GameManager.Instance.IsPlayerMoving)//如果游戏状态是玩家移动且玩家不是正在移动
        {
            if (Input.GetMouseButtonDown(0)&&!EventSystem.current.IsPointerOverGameObject())//如果点击了鼠标左键且鼠标不在UI上
            {
                EventHandler.CallMouseLeftClick(CameraManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition));//调用鼠标点击左键事件
            }
        }
    }
}
