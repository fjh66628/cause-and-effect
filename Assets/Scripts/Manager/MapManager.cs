using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MapManager : MonoBehaviour//这个脚本管理地图中的坐标
{
    MapCell[,] mapGrid;//地图的网格数组
    public MapCell[,] getMapGrid => mapGrid;//地图的网格数组
    float cellWidth = 1.5f;//网格的宽度
    float cellHeight = 0.5f;//网格的高度
    Vector2 i = new Vector2(0.75f, 0.375f);//网格横向基向量
    Vector2 j = new Vector2(0.75f, -0.375f);//网格纵向基向量
    Vector2 basePoint = new Vector2(0.75f, 0);//基准相对点坐标.
    public float getCellWidth => cellWidth;//获取网格的宽度
    public float getCellHeight => cellHeight;//获取网格的高度
    Vector2 zeroPoint;//网格零点坐标
    LevelManagement levelManagement;//关卡管理
    [Header("玩家位置")]
    [SerializeField] Vector2Int playerPosition = new Vector2Int(0, 0);//玩家位置
    void OnEnable()
    {
        EventHandler.levelLoaded += UpdateMapInfo;//注册关卡加载事件
    }
    void OnDisable()
    {
        EventHandler.levelLoaded -= UpdateMapInfo;//注销关卡加载事件
    }


    /*需要关卡加载逻辑，当关卡加载时，需要更新地图信息*/
    public void UpdateMapInfo()
    {
        if (LevelManager.Instance == null || GameManager.Instance == null)
        {
            Debug.LogError("LevelManager 或 GameManager 实例为空");
            return;
        }

        levelManagement = LevelManager.Instance.GetLevelManagement(GameManager.Instance.getChapterNumber, GameManager.Instance.getLevelNumber);
        bool isCollapse = levelManagement.getIsCollapse;
        if (isCollapse)
        {
            Debug.Log("当前关卡为塌陷关卡");
        }
        else
        {
            Debug.Log("当前关卡为普通关卡");
        }
        if (levelManagement == null)
        {
            Debug.LogError("获取关卡管理失败");
            return;
        }
        // 初始化地图网格数组
        mapGrid = new MapCell[levelManagement.getMapSize.x, levelManagement.getMapSize.y];

        // 1. 先完整初始化整个数组
        for (int x = 0; x < mapGrid.GetLength(0); x++)
        {
            for (int y = 0; y < mapGrid.GetLength(1); y++)
            {
                // 确保每个位置都有 MapCell 对象
                if (mapGrid[x, y] == null)
                {
                    mapGrid[x, y] = new MapCell();
                }
                // 先全部设为 None
                mapGrid[x, y].setCellContent(!isCollapse ? MapCellContent.None : MapCellContent.Door_singleuse);
                mapGrid[x, y].SetId("0");
            }
        }


        // 3. 其余代码保持不变...
        playerPosition = levelManagement.getPlayerStartPosition;

        mapGrid[playerPosition.x, playerPosition.y].setCellContent(MapCellContent.Start);

        for (int x = 0; x < levelManagement.getUnitPositions.Count; x++)
        {
            Vector2Int pos = levelManagement.getUnitPositions[x].getPosition;
            MapCellContent content = levelManagement.getUnitPositions[x].getMapCellContent;

            // 确保坐标在数组范围内
            if (pos.x >= 0 && pos.x < 12 && pos.y >= 0 && pos.y < 12)
            {
                mapGrid[pos.x, pos.y].setCellContent(content);
                mapGrid[pos.x, pos.y].SetId(levelManagement.getUnitPositions[x].getId);
            }
            else
            {
                Debug.LogError($"单位坐标超出范围: {pos}");
            }
        }
    }


    public Vector2Int MoveDrection(Vector2 mousePosition, Vector2 playerPosition)//通过鼠标坐标获取移动方向，玩家的移动方式为网格移动
    {
        Vector2 direction = mousePosition - playerPosition;
        return math.abs(Vector2.Dot(direction, i)) > math.abs(Vector2.Dot(direction, j)) ? new Vector2Int((int)math.sign(Vector2.Dot(direction, i)), 0) : new Vector2Int(0, (int)math.sign(Vector2.Dot(direction, j)));
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
        if (zeroPoint == Vector2.zero)
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
        return GetWorldPosition(playerPosition);
    }

    public Vector2 GetWorldPosition(Vector2Int gridPosition)//通过网格坐标获取世界坐标
    {
        return zeroPoint + basePoint + (float)gridPosition.x * i + (float)gridPosition.y * j;
    }
    public Vector2 GetWorldPosition(MapCellContent mapCellContent)//通过网格内容获取世界坐标
    {
        return GetWorldPosition(FindGridPosition(mapCellContent)[0]);

    }
    public Vector2 GetWorldPosition(MapCellContent mapCellContent, int i)//通过网格内容获取世界坐标
    {
        return GetWorldPosition(FindGridPosition(mapCellContent)[i]);

    }

    public List<Vector2Int> FindGridPosition(MapCellContent mapCellContent)//查找在地图网格中的坐标
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int m = 0; m < mapGrid.GetLength(0); m++)
        {
            for (int n = 0; n < mapGrid.GetLength(1); n++)
            {
                if (mapGrid[m, n].getCellContent == mapCellContent)
                {
                    positions.Add(new Vector2Int(m, n));
                }
            }
        }
        return positions;
    }


    public void ChangePlayerPosition(Vector2Int position)//改变玩家在地图中的坐标
    {
        if (playerPosition == new Vector2Int(-1, -1))
        {
            Debug.LogError("玩家在地图中不存在");
            return;
        }
        if (position.x + playerPosition.x < 0 || position.x + playerPosition.x >= mapGrid.GetLength(0) || position.y + playerPosition.y < 0 || position.y + playerPosition.y >= mapGrid.GetLength(1))
        {
            LeaveTheMap();
            return;
        }
        if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.None || mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Start)
        {
            PlayerMove(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.End)
        {
            ReachTheEnd();
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Wall)
        {
            ToTheWall(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Door_singleuse)
        {
            ToTheDoorSingleUse(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Collapse)
        {
            ToTheCollapse(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Wall_unbreakable)
        {
            ToTheWallUnbreakable(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Water)
        {
            ToTheWater(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Key)
        {
            ToTheKey(position);
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Door_locked)
        {
            ToTheDoorClosed();
        }
        else if (mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getCellContent == MapCellContent.Door_opened)
        {
            ToTheDoorOpened(position);
        }
        else
        {
            Debug.LogWarning("玩家移动到了已有物品的单元格");
        }

    }

    void PlayerMove(Vector2Int position)//玩家移动到新的单元格
    {
        MapCell playerCell = mapGrid[playerPosition.x, playerPosition.y];
        playerPosition = position + playerPosition;//将玩家移动到新的单元格 
        EventHandler.CallPlayerMove();
    }


    void LeaveTheMap()//玩家离开地图
    {
        Debug.LogWarning("玩家移动到了地图外部");
    }
    void ReachTheEnd()//玩家到达目标单元格
    {
        if (GameManager.Instance.ReachTheEnd() == false)//玩家到达目标单元格
        {
            StartCoroutine(PlayerPositionReset());//玩家重置位置到起始位置
        }
        Debug.Log("玩家到达了目标单元格");
    }

    IEnumerator PlayerPositionReset()//踏上轮回之后玩家重置位置到起始位置
    {
        yield return new WaitForSeconds(0.7f);
        playerPosition = levelManagement.getPlayerStartPosition;
        FindObjectOfType<PlayerMove>().OnLevelLoaded();//改变玩家在地图中的坐标
    }


    void LoadAnimate()//回到起始位置动画
    {
        Debug.Log("回到起始位置动画");
    }

    void ToTheWall(Vector2Int position)//玩家到达了墙单元格
    {

        if (GameManager.Instance.getPlayerState == PlayerState.BreakWall)//如果玩家可以破坏墙单元格
        {
            EventHandler.CallBreakTheWall();//调用玩家破坏墙单元格事件
            StartCoroutine(BreakTheWallAnimation(position));//玩家破坏墙单元格动画
        }
        else if (GameManager.Instance.getPlayerState == PlayerState.PassWall)//如果玩家可以穿墙单元格
        {
            PlayerMove(position);//玩家移动到新的单元格
            EventHandler.CallPassTheWall(playerPosition);//调用玩家穿墙单元格事件
        }
        else
        {
            Debug.LogWarning("玩家到达了墙单元格");
        }
    }
    void ToTheDoorSingleUse(Vector2Int position)//玩家到达了一次性门单元格
    {
        PlayerMove(position);//玩家移动到新的单元格
        if (GameManager.Instance.getPlayerState != PlayerState.Fly)//如果玩家可以破坏墙单元格
        {
            mapGrid[playerPosition.x, playerPosition.y].setCellContent(MapCellContent.Collapse);//将一次性门单元格设置为塌陷单元格
            EventHandler.CallChangeItem(MapCellContent.Collapse, playerPosition);//调用改变物品事件
            Debug.LogWarning("玩家到达了一次性门单元格,地面塌陷给予提示，危险快走");
        }
        Debug.LogWarning("玩家通过了一次性门单元格");
    }
    void ToTheCollapse(Vector2Int position)//玩家到达了塌陷单元格
    {
        if (GameManager.Instance.getPlayerState == PlayerState.Fly)//如果玩家是飞行状态
        {
            PlayerMove(position);//玩家移动到新的单元格
        }
        else
        {
            Debug.LogWarning("玩家不能到达塌陷单元格");
        }
    }
    void ToTheWallUnbreakable(Vector2Int position)//玩家到达了不可破坏的墙单元格
    {
        if (GameManager.Instance.getPlayerState == PlayerState.PassWall)//如果玩家可以穿墙单元格
        {
            PlayerMove(position);//玩家移动到新的单元格
            EventHandler.CallPassTheWall(playerPosition);//调用玩家穿墙单元格事件
        }
        else
        {
            Debug.LogWarning("玩家到达了不可破坏的墙单元格");
        }
    }
    void ToTheWater(Vector2Int position)//玩家到达了水单元格
    {
        if (GameManager.Instance.getPlayerState == PlayerState.Fly)//如果玩家是飞行状态
        {
            PlayerMove(position);//玩家移动到新的单元格
        }
        else
        {
            Debug.LogWarning("玩家不能到达水单元格");
        }
    }
    void ToTheKey(Vector2Int position)//玩家到达了钥匙单元格
    {
        ChangeTheDoorState(mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].getId);//改变门单元格的状态
        PlayerMove(position);//玩家移动到新的单元格
    }
    void ToTheDoorClosed()//玩家到达了关闭的门单元格
    {
        Debug.LogWarning("玩家到达了关闭的门单元格");
    }
    void ToTheDoorOpened(Vector2Int position)//玩家到达了开启的门单元格
    {
        PlayerMove(position);//玩家移动到新的单元格
        Debug.LogWarning("玩家通过了开启的门单元格");
    }

    IEnumerator BreakTheWallAnimation(Vector2Int position)//玩家破坏墙单元格动画
    {
        mapGrid[position.x + playerPosition.x, position.y + playerPosition.y].setCellContent(MapCellContent.None);//将墙单元格设置为空单元格
        ItemRenderManager.Instance.UnloadItemById($"Wall_{position.x + playerPosition.x}_{position.y + playerPosition.y}");//将墙单元格中的物品移除
        Debug.Log($"玩家破坏了墙单元格动画");
        yield return new WaitForSeconds(0.2f);//等待0.2秒

    }

    void ChangeTheDoorState(string keyId)//改变门单元格的状态
    {
        for (int i = 0; i < mapGrid.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrid.GetLength(1); j++)
            {
                if (mapGrid[i, j].getId == keyId)
                {
                    if (mapGrid[i, j].getCellContent == MapCellContent.Door_locked)
                    {
                        EventHandler.CallChangeItem(MapCellContent.Door_opened, new Vector2Int(i, j));//调用改变物品事件
                        mapGrid[i, j].setCellContent(MapCellContent.Door_opened);//将关闭的门单元格设置为开启的门单元格
                        Debug.LogWarning("玩家改变了关闭的单元的门单元格");
                    }
                    else if (mapGrid[i, j].getCellContent == MapCellContent.Door_opened)
                    {
                        EventHandler.CallChangeItem(MapCellContent.Door_locked, new Vector2Int(i, j));//调用改变物品事件
                        mapGrid[i, j].setCellContent(MapCellContent.Door_locked);//将开启的门单元格设置为关闭的门单元格
                        Debug.LogWarning("玩家改变了开启的单元的门单元格");
                    }
                }
            }
        }
    }
    public void CheckPlayerPosition()//判断玩家在哪个位置
    {
        if (mapGrid[playerPosition.x, playerPosition.y].getCellContent == MapCellContent.Collapse)//如果玩家在塌陷单元格
        {
            GameOver("完蛋，你掉下去了");
        }
        else if (mapGrid[playerPosition.x, playerPosition.y].getCellContent == MapCellContent.Water)//如果玩家在水单元格
        {
            GameOver("你被水包围了");
        }
        else if (mapGrid[playerPosition.x, playerPosition.y].getCellContent == MapCellContent.Wall)//如果玩家在墙单元格
        {
            GameOver("你准备在墙里边游泳");
        }
        else if (mapGrid[playerPosition.x, playerPosition.y].getCellContent == MapCellContent.Wall_unbreakable)//如果玩家在钥匙单元格
        {
            GameOver("你卡在了坚固的墙里边");
        }

    }
    void GameOver(string reason)//游戏失败
    {
        EventHandler.CallGameOver(reason);//调用游戏失败事件
    }
}