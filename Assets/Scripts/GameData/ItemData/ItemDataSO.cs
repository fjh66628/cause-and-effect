using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu( fileName = "ItemData", menuName = "GameData/ItemData", order = 1 )]
public class ItemDataSO : ScriptableObject
{
    [SerializeField]private List<ItemData> itemList;
    public List<ItemData> getItemList => itemList;
}
