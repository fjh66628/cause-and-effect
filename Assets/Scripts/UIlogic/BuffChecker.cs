using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[System.Serializable]

public class BuffChecker : MonoBehaviour
{
    [SerializeField] private Image buffPanel;
    [SerializeField] private TextMeshProUGUI buffText;
    public void OnPointerEnter()
    {
        buffPanel.gameObject.SetActive(true);
        string buff = GameManager.Instance.GetBuffText();
        buffText.text = buff;
    }
    public void OnPointerExit()
    {
        buffPanel.gameObject.SetActive(false);
    }
}
