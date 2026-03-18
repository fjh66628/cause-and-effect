using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerMove : MonoBehaviour
{
    [Header("玩家移动参数")]
    [SerializeField] private float moveDuration = 0.2f;//移动时间
    private MapManager mapManager;
    private void OnEnable() 
    {
        EventHandler.onMouseLeftClick += OnMouseLeftClick;//订阅鼠标点击左键事件
        EventHandler.mapLoaded += OnLevelLoaded;//订阅关卡加载事件
    }
    private void OnDisable()
    {
        EventHandler.onMouseLeftClick -= OnMouseLeftClick;//取消订阅鼠标点击左键事件
        EventHandler.mapLoaded -= OnLevelLoaded;//取消订阅关卡加载事件
    }

    private void OnLevelLoaded()
    {
        mapManager = FindObjectOfType<MapManager>();//获取地图管理器组件
        if(mapManager == null)
        {
            Debug.LogError("地图管理器组件不存在");
            return;
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//初始化玩家在世界坐标中的位置
    }

    IEnumerator ChangePosition(Vector3 targetPosition)
    {
        yield return null;//等待一帧
        GameManager.Instance.SetIsPlayerMoving(true);//设置玩家是正在移动
        float elapsedTime = 0f;//已用时间
        Vector3 startPosition = transform.position;//起始位置
        Debug.Log($"动画效果");
        while(elapsedTime < moveDuration)
        {
            elapsedTime += Time.fixedDeltaTime;//更新已用时间
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);//插值计算新位置
            yield return new WaitForFixedUpdate();//等待固定时间步长
        }
        GameManager.Instance.SetIsPlayerMoving(false);//设置玩家不是正在移动
    }

    private void OnMouseLeftClick(Vector2 mousePosition)
    {
        Vector2Int direction = mapManager.MoveDrection(mousePosition,transform.position);//获取玩家移动方向
        if(direction != Vector2.zero)
        {
            mapManager.ChangePlayerPosition(direction);//改变玩家在地图中的坐标
        }
        StartCoroutine(ChangePosition(mapManager.GetPlayerWorldPosition()));//改变玩家在世界坐标中的位置
    }
}
