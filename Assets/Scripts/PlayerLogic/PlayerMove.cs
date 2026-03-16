using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private MapManager mapManager;
    private void OnEnable() 
    {
        EventHandler.onMouseLeftClick += OnMouseLeftClick;//订阅鼠标点击左键事件
    }
    private void OnDisable()
    {
        EventHandler.onMouseLeftClick -= OnMouseLeftClick;//取消订阅鼠标点击左键事件
    }
    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();//获取地图管理器组件
        if(mapManager == null)
        {
            Debug.LogError("地图管理器组件不存在");
            return;
        }
        transform.position = mapManager.GetPlayerWorldPosition();//初始化玩家在世界坐标中的位置
    }
    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        Vector2Int direction = mapManager.MoveDrection(mousePosition,transform.position);//获取玩家移动方向
        if(direction != Vector2.zero)
        {
            mapManager.ChangePlayerPosition(direction);//改变玩家在地图中的坐标
        }
        transform.position = mapManager.GetPlayerWorldPosition();//改变玩家在世界坐标中的位置
    }
}
