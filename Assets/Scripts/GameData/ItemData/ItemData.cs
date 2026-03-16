using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MapCellContentData
{
    [SerializeField]private MapCellContent mapCellContent;//地图网格单元格内容
    [SerializeField]private string detail;
    [SerializeField]private Sprite contentIcon;
    public MapCellContent getMapCellContent => mapCellContent;//获取地图网格单元格内容
    public string getDetail => detail;
    public Sprite getItemIcon =>contentIcon;
}
