using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardData", menuName = "GameData/CardData", order = 1)]
public class CardDataSO : ScriptableObject
{
    [SerializeField] private List<CardData> cardList;
    public List<CardData> getCardList => cardList;
}
