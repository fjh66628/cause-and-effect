using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu( fileName = "ObjectData", menuName = "GameData/ObjectData", order = 1 )]
public class ItemDataSO : ScriptableObject
{
    [SerializeField]private List<MapCellContentData> mapCellContentList;
    public List<MapCellContentData> getMapCellContentList => mapCellContentList;
}
