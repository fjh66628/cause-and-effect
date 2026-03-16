using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ItemRenderManager : SingletonMono<ItemRenderManager>
{
    [System.Serializable]
    public class prefabMapping//储存物品预制体的数据结构
    {
        [SerializeField]private MapCellContent mapCellContent;//次物品的种类
        [SerializeField]private GameObject itemPrefab;//次物品的预制体
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
    [SerializeField]private List<prefabMapping> itemPrefabs;//物品预制体列表

    Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();//物品数据字典
    MapManager mapManager;
    void OnEnable()
    {
        EventHandler.levelLoaded += LoadAllItems;
    }
    void OnDisable()
    {
        EventHandler.levelLoaded -= LoadAllItems;
    }
    void Start()
    {
        LoadAllItems();
    }

    void LoadAllItems()
    {
        mapManager = FindObjectOfType<MapManager>();
        if(mapManager == null)
        {
            Debug.LogError("MapManager not found");
            return;
        }
        for(int x = 0; x < mapManager.getMapGrid.GetLength(0); x++)
        {
            for(int y = 0; y < mapManager.getMapGrid.GetLength(1); y++)
            {
                MapCellContent mapCellContent = mapManager.getMapGrid[x,y].getCellContent;
                if(mapCellContent != MapCellContent.None)
                {
                    CreatItem(mapCellContent);
                    //渲染物品
                }
            }
        }
        //加载所有物品
    }
    void CreatItem(MapCellContent mapCellContent)//根据物品种类创建物品
    {
        prefabMapping prefabMapping = itemPrefabs.Find(x => x.getMapCellContent == mapCellContent);
        if(prefabMapping == null)
        {
            Debug.LogError("物品" + mapCellContent + "的预制体未找到");
            return;
        }
        GameObject item = Instantiate(prefabMapping.getItemPrefab, mapManager.GetWorldPosition(mapCellContent), Quaternion.identity);
        item.name = mapCellContent.ToString();
        string itemID = GetItemID(mapCellContent, mapManager.FindGridPosition(mapCellContent));
        itemDatas.Add(itemID, new ItemData(itemID,item));
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
                Debug.Log($"卸载物品: {itemId}");
            }
            
            itemDatas.Remove(itemId);
            return true;
        }
        
        Debug.LogWarning($"未找到物品ID: {itemId}");
        return false;
    }
}
