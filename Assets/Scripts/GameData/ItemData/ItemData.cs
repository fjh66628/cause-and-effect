using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemData
{
    [SerializeField]private string itemName;
    [SerializeField]private string itemDetail;
    [SerializeField]private Sprite itemIcon;
    public string getItemName => itemName;
    public string getItemDetail => itemDetail;
    public Sprite getItemIcon => itemIcon;
}
