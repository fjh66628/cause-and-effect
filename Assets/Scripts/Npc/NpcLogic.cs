using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NpcLogic : MonoBehaviour
{
    [SerializeField] bool isTalked = false;
    [SerializeField] DialogueSO dialogueSO;
    [SerializeField] Vector2Int talkPosition;
    [Header("第几周目会出现会话（从1开始）")]
    [SerializeField] int count = 1;//第几周目会话
    void FixedUpdate()
    {
        Vector2Int playerPosition = new Vector2Int(FindObjectOfType<MapManager>().getPlayerPosition.y, FindObjectOfType<MapManager>().getPlayerPosition.x);
        if (playerPosition == talkPosition && !isTalked && GameManager.Instance.getEndStepCount == count)
        {
            isTalked = true;
            EventHandler.showDialogue(dialogueSO);
        }
    }
}
