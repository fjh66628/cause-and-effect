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

    void LoadAllItems()
    {
        StartCoroutine(LoadItem());
    }

    IEnumerator LoadItem()//加载物品
    {
        itemDatas.Clear();
        yield return null;
        mapManager = FindObjectOfType<MapManager>();
        if(mapManager == null)
        {
            Debug.LogError("MapManager not found");
        }
        foreach(MapCellContent mapCellContent in System.Enum.GetValues(typeof(MapCellContent)))
        {
            if(mapCellContent != MapCellContent.None)
            {
                CreatItem(mapCellContent);
                //渲染物品
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
        List<Vector2Int> itemsPositions=mapManager.FindGridPosition(mapCellContent);
        foreach(Vector2Int position in itemsPositions)
        {
            if(mapManager.getMapGrid[position.x, position.y].getId == "0"||mapManager.getMapGrid[position.x, position.y].getId == "")
            {    

                GameObject item = Instantiate(prefabMapping.getItemPrefab, mapManager.GetWorldPosition(position), Quaternion.identity);
                string itemID = GetItemID(mapCellContent, position);
                itemDatas.Add(itemID, new ItemData(itemID,item));
            }
            else
            {

                GameObject item = Instantiate(prefabMapping.getItemPrefab, mapManager.GetWorldPosition(position), Quaternion.identity);
                SetItemDefaultColor(item, mapManager.getMapGrid[position.x, position.y].getId);
                string itemID = GetItemID(mapCellContent, position);
                itemDatas.Add(itemID, new ItemData(itemID,item));
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

        SetItemColor(item,defaultColor);
    }
    Color GetDefaultColor(string itemID)//获取物品默认颜色
    {
        if (itemID.Length != 3)
        {
            Debug.LogError("物品 ID" +itemID+" 格式错误，必须为3位");
            return Color.white;
        }
        float r = int.Parse(itemID[0].ToString());
        float g = int.Parse(itemID[1].ToString());
        float b = int.Parse(itemID[2].ToString());
        return new Color((155f+r*10)/255f,(155f+g*10)/255f , (155f+g*10f)/255f);
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
}
