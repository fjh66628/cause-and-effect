using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapCellContent//地图网格单元格内容
{
    None,//无内容
    Wall,//墙
    Floor,//地板
    Door,//门
    Key,//钥匙
    Player,//玩家
    Enemy,//敌人
}


public class MapCell//地图网格单元格数据
{
    Stack<int> step=new Stack<int>();//到达该单元格的步数
    MapCellContent cellContent = MapCellContent.None;//单元格内容
    public int getStep => step.Peek();//获取到达该单元格的步数
    public MapCellContent getCellContent => cellContent;//获取单元格内容
    public void setCellContent(MapCellContent cellContent)//设置单元格内容
    {
        this.cellContent = cellContent;
    }
    public void setStep(int step)//设置到达该单元格的步数
    {
        this.step.Push(step);
    }
}