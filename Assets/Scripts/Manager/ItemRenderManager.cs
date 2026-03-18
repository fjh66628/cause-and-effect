using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ItemRenderManager : SingletonMono<ItemRenderManager>
{
    [System.Serializable]
    public class prefabMapping//储存物品预制体的数据结构
    {
        [SerializeField] private MapCellContent mapCellContent;//次物品的种类
        [SerializeField] private GameObject itemPrefab;//次物品的预制体
        public MapCellContent getMapCellContent => mapCellContent;//获取物品的种类
        public GameObject getItemPrefab => itemPrefab;//获取物品的预制体
    }

    [System.Serializable]
    public class ItemData//储存物品数据的数据结构
    {
        string itemID;
        GameObject itemObject;
        public ItemData(string itemID, GameObject itemObject)//物品数据构造函数
        {
            this.itemID = itemID;
            this.itemObject = itemObject;
        }
        public string getItemID => itemID;//获取物品的ID
        public GameObject getItemObject => itemObject;//获取物品的游戏对象
    }

    [Header("物品预制体")]
    [SerializeField] private List<prefabMapping> itemPrefabs;//物品预制体列表

    Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();//物品数据字典
    MapManager mapManager;
    void OnEnable()
    {
        EventHandler.changeItem += ChangeItem;//注销改变物品事件
        EventHandler.levelLoaded += LoadAllItems;
        EventHandler.passTheWall += PassTheWall;//注销玩家穿墙单元格事件
    }
    void OnDisable()
    {
        EventHandler.changeItem -= ChangeItem;//注销改变物品事件
        EventHandler.levelLoaded -= LoadAllItems;
        EventHandler.passTheWall -= PassTheWall;//注销玩家穿墙单元格事件
    }

    void LoadAllItems()
    {
        StartCoroutine(LoadItem());
    }

    IEnumerator LoadItem()//加载物品
    {
        itemDatas.Clear();
        yield return null;
        mapManager = FindObjectOfType<MapManager>();
        if (mapManager == null)
        {
            Debug.LogError("MapManager not found");
        }
        foreach (MapCellContent mapCellContent in System.Enum.GetValues(typeof(MapCellContent)))
        {
            if (mapCellContent != MapCellContent.None)
            {
                CreatItem(mapCellContent);
                //渲染物品
            }
        }
        //加载所有物品
    }
    void PassTheWall(Vector2Int position)//玩家穿墙单元格事件
    {
        if (itemDatas.TryGetValue(GetItemID(MapCellContent.Wall, position), out ItemData itemData))
        {
            Color color = itemData.getItemObject.GetComponent<SpriteRenderer>().color;
            itemData.getItemObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.5f);
        }
        if (itemDatas.TryGetValue(GetItemID(MapCellContent.Wall_unbreakable, position), out itemData))
        {
            Color color = itemData.getItemObject.GetComponent<SpriteRenderer>().color;
            itemData.getItemObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }
    void CreatItem(MapCellContent mapCellContent)//根据物品种类创建物品
    {
        prefabMapping prefabMapping = itemPrefabs.Find(x => x.getMapCellContent == mapCellContent);
        if (prefabMapping == null)
        {
            Debug.LogError("物品" + mapCellContent + "的预制体未找到");
            return;
        }
        List<Vector2Int> itemsPositions = mapManager.FindGridPosition(mapCellContent);
        foreach (Vector2Int position in itemsPositions)
        {
            if (mapManager.getMapGrid[position.x, position.y].getId == "0" || mapManager.getMapGrid[position.x, position.y].getId == "")
            {

                GameObject item = Instantiate(prefabMapping.getItemPrefab, mapManager.GetWorldPosition(position), Quaternion.identity);
                string itemID = GetItemID(mapCellContent, position);
                itemDatas.Add(itemID, new ItemData(itemID, item));
            }
            else
            {

                GameObject item = Instantiate(prefabMapping.getItemPrefab, mapManager.GetWorldPosition(position), Quaternion.identity);
                SetItemDefaultColor(item, mapManager.getMapGrid[position.x, position.y].getId);
                string itemID = GetItemID(mapCellContent, position);
                itemDatas.Add(itemID, new ItemData(itemID, item));
            }
        }
    }



    string GetItemID(MapCellContent mapCellContent, Vector2Int position)//根据物品种类获取物品ID
    {
        return $"{mapCellContent}_{position.x}_{position.y}";
    }
    public bool UnloadItemById(string itemId)
    {
        if (itemDatas.TryGetValue(itemId, out ItemData itemData))
        {
            if (itemData.getItemObject != null)
            {
                Destroy(itemData.getItemObject);

            }

            itemDatas.Remove(itemId);
            return true;
        }

        Debug.LogWarning($"未找到物品ID: {itemId}");
        return false;
    }

    void SetItemDefaultColor(GameObject item, string itemID)//设置物品默认颜色
    {
        Color defaultColor = GetDefaultColor(itemID);

        SetItemColor(item, defaultColor);
    }
    Color GetDefaultColor(string itemID)//获取物品默认颜色
    {
        if (itemID.Length != 3)
        {
            return Color.white;
        }
        float r = int.Parse(itemID[0].ToString());
        float g = int.Parse(itemID[1].ToString());
        float b = int.Parse(itemID[2].ToString());
        return new Color((155f + r * 10) / 255f, (155f + g * 10) / 255f, (155f + g * 10f) / 255f);
    }
    void SetItemColor(GameObject item, Color color)//设置物品颜色
    {
        if (item == null) return;

        // 设置当前物体的SpriteRenderer颜色
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        // 递归设置所有子物体
        foreach (SpriteRenderer childSpriteRenderer in item.GetComponentsInChildren<SpriteRenderer>())
        {
            childSpriteRenderer.color = color;

        }
    }

    void ChangeItem(MapCellContent mapCellContent, Vector2Int position)//改变物品事件 
    {
        // 1. 检查当前存在的物品状态
        string itemId = GetItemID(mapCellContent, position);

        // 2. 根据当前状态决定切换方向
        if (itemDatas.ContainsKey(itemId))
        {
            // 当前是锁定状态，切换到开启状态
            ChangeItemState(position, mapCellContent);
        }
        else
        {
            Debug.LogWarning($"在位置 {position} 未找到物品对象");
        }
    }
    void ChangeItemState(Vector2Int position, MapCellContent fromState)
    {
        string fromId = GetItemID(fromState, position);
        MapCellContent toState = fromState;
        if (fromState == MapCellContent.Door_locked)
        {
            toState = MapCellContent.Door_opened;
        }
        else if (fromState == MapCellContent.Door_opened)
        {
            toState = MapCellContent.Door_locked;
        }
        else if (fromState == MapCellContent.Door_singleuse)
        {
            toState = MapCellContent.Collapse;
        }
        // 1. 卸载旧物品
        if (UnloadItemById(fromId))
        {
            Debug.Log($"成功卸载 {fromState} 物品");
        }

        // 2. 创建新物品    
        prefabMapping newItemPrefab = itemPrefabs.Find(x => x.getMapCellContent == toState);
        if (newItemPrefab != null)
        {
            string toId = GetItemID(toState, position);
            GameObject newItem = Instantiate(newItemPrefab.getItemPrefab, mapManager.GetWorldPosition(position), Quaternion.identity);

            // 3. 设置颜色（根据地图网格中的ID）
            string colorId = mapManager.getMapGrid[position.x, position.y].getId;
            SetItemDefaultColor(newItem, colorId);

            // 4. 添加到字典
            itemDatas.Add(toId, new ItemData(toId, newItem));

            Debug.Log($"成功创建 {toState} 物品，颜色ID: {colorId}");
        }
        else
        {
            Debug.LogError($"未找到 {toState} 的预制体");
        }

    }
}
