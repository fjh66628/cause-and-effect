using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maprender : MonoBehaviour
{
    private MapManager mapManager;
    private void Start()
    {
        mapManager = FindObjectOfType<MapManager>();//获取地图管理器组件
        if(mapManager == null)
        {
            Debug.LogError("地图管理器组件不存在");
            return;
        }
        mapManager.SetZeroPoint(transform.position);//设置地图管理器零点坐标
        EventHandler.CallMapLoaded();//调用地图加载事件
    }
}
