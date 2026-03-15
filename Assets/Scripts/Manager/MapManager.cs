using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MapManager : SingletonMono<MapManager>//这个脚本管理地图中的坐标
{
    MapCell[,] mapGrid = new MapCell[10, 10];
    public MapCell[,] getMapGrid => mapGrid;//地图的网格数组
    float cellWidth = 1.5f;//网格的宽度
    float cellHeight = 0.5f;//网格的高度
    Vector2 i = new Vector2(0.75f, 0.375f);//网格横向基向量
    Vector2 j = new Vector2(0.75f, -0.375f);//网格纵向基向量
    Vector2 basePoint = new Vector2(0.75f, 0);//基准相对点坐标.
    public float getCellWidth => cellWidth;//获取网格的宽度
    public float getCellHeight => cellHeight;//获取网格的高度
    Vector2 zeroPoint;//网格零点坐标
    [Header("设置玩家初始位置")]
    [SerializeField] Vector2Int playerPosition = new Vector2Int(0, 0);//玩家初始位置
    protected override void InitializeSingleton()
    {
        for(int x = 0; x < mapGrid.GetLength(0); x++)
        {
            for(int y = 0; y < mapGrid.GetLength(1); y++)
            {
                mapGrid[x, y] = new MapCell();
            }
        }
        playerPosition=LevelManager.Instance.getPlayerStartPosition;//获取玩家初始位置
        mapGrid[playerPosition.x, playerPosition.y].setCellContent(MapCellContent.Player);//设置玩家初始位置
        mapGrid[playerPosition.x, playerPosition.y].setStep(0);//设置玩家初始位置的步数为0

    }

    public Vector2Int MoveDrection(Vector2 mousePosition , Vector2 playerPosition)//通过鼠标坐标获取移动方向，玩家的移动方式为网格移动
    {
        Vector2 direction = mousePosition - playerPosition;
        return math.abs(Vector2.Dot(direction, i)) > math.abs(Vector2.Dot(direction, j)) ? new Vector2Int((int)math.sign(Vector2.Dot(direction, i)), 0)  : new Vector2Int(0, (int)math.sign(Vector2.Dot(direction, j)));    
    }

    public void SetZeroPoint(Vector2 zeroPoint)//设置网格零点坐标
    {
        this.zeroPoint = zeroPoint;
    }

    // 针对斜角坐标系的最近网格点查找算法 l = m*i + n*j+e，其中e为误差向量
    public Vector2Int FindNearestGridPoint(Vector2 position)//通过坐标获取最近的网格坐标
    {
        // 步骤1: 计算理论上的m,n值（使用线性代数）
        float determinant = i.x * j.y - i.y * j.x;
        if(zeroPoint == Vector2.zero)
        {
            Debug.LogError("网格零点坐标未设置");
            return Vector2Int.zero;
        }
        Vector2 l = position - zeroPoint - basePoint;//参考系转为地图的（0,0）点
        if (Mathf.Abs(determinant) < 0.001f) return Vector2Int.zero;
        
        // 解线性方程组: l = m*i + n*j
        float m_exact = (l.x * j.y - l.y * j.x) / determinant;
        float n_exact = (i.x * l.y - i.y * l.x) / determinant;
        
        // 步骤2: 四舍五入得到初始候选点
        int m_round = Mathf.RoundToInt(m_exact);
        int n_round = Mathf.RoundToInt(n_exact);
        
        // 步骤3: 在3x3邻域内搜索最优解
        Vector2Int bestGrid = new Vector2Int(m_round, n_round);
        float minError = float.MaxValue;
        
        // 搜索范围：中心点及其周围8个点
        for (int dm = -1; dm <= 1; dm++)
        {
            for (int dn = -1; dn <= 1; dn++)
            {
                int test_m = m_round + dm;
                int test_n = n_round + dn;
                
                // 计算网格点坐标
                Vector2 gridPoint = test_m * i + test_n * j;
                
                // 计算误差向量的平方长度（避免开方运算，提高性能）
                Vector2 error = l - gridPoint;
                float errorSqr = error.x * error.x + error.y * error.y;
                
                if (errorSqr < minError)
                {
                    minError = errorSqr;
                    bestGrid = new Vector2Int(test_m, test_n);
                }
            }
        }
        
        return bestGrid;
    }
    public Vector2 GetPlayerWorldPosition()//获取玩家在世界坐标中的位置
    {
        return GetWorldPosition(FindPlayer());
    }
    
    Vector2 GetWorldPosition(Vector2Int gridPosition)//通过网格坐标获取世界坐标
    {
        return zeroPoint + basePoint + (float)gridPosition.x * i + (float)gridPosition.y * j;
    }
    Vector2Int FindPlayer()//查找玩家在地图网格中的坐标
    {
        for (int m = 0; m < mapGrid.GetLength(0); m++)
        {
            for (int n = 0; n < mapGrid.GetLength(1); n++)
            {
                if (mapGrid[m, n].getCellContent == MapCellContent.Player)
                {
                    playerPosition = new Vector2Int(m, n);
                    return playerPosition;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }


    public void ChangePlayerPosition(Vector2Int position)//改变玩家在地图中的坐标
    {
        Vector2Int playerPosition = FindPlayer();//查找玩家在地图网格中的坐标
        if(playerPosition == new Vector2Int(-1, -1))
        {
            Debug.LogError("玩家在地图中不存在");
            return;
        }
        if(position.x + playerPosition.x < 0 || position.x + playerPosition.x >= mapGrid.GetLength(0) || position.y + playerPosition.y < 0 || position.y + playerPosition.y >= mapGrid.GetLength(1))
        {
            Debug.LogWarning("玩家移动到了地图外部");
            return;
        }
        if(mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.None)
        {
            MapCell playerCell = mapGrid[playerPosition.x, playerPosition.y];
            playerCell.setCellContent(MapCellContent.None);//将玩家所在单元格内容设置为空
            mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].setCellContent(MapCellContent.Player);//将玩家移动到新的单元格
            mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].setStep(playerCell.getStep + 1);//将玩家移动到新的单元格的步数设置为玩家所在单元格的步数加一
            Debug.Log("玩家移动到了" + (position.x + playerPosition.x) + "," + (position.y + playerPosition.y));
        }
        else
        {
            Debug.LogWarning("玩家移动到了已有物品的单元格");
        }
    
    }
}