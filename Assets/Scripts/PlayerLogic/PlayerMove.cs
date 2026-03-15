using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
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
        transform.position = MapManager.Instance.GetPlayerWorldPosition();//初始化玩家在世界坐标中的位置
    }
    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        Vector2Int direction = MapManager.Instance.MoveDrection(mousePosition,transform.position);//获取玩家移动方向
        Debug.Log(direction);
        if(direction != Vector2.zero)
        {
            MapManager.Instance.ChangePlayerPosition(direction);//改变玩家在地图中的坐标
        }
        transform.position = MapManager.Instance.GetPlayerWorldPosition();//改变玩家在世界坐标中的位置
    }
}
