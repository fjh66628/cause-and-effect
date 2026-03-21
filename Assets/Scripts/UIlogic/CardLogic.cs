using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class CardLogic : MonoBehaviour
{
    [SerializeField] private CardData cardData;

    public void SetCardData(CardData cardData)
    {
        this.cardData = cardData;
    }

}
