using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maprender : MonoBehaviour
{
    void Awake()
    {
        MapManager.Instance.SetZeroPoint(transform.position);//设置地图管理器零点坐标
    }
}
