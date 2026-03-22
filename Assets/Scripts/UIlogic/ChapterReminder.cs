using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ChapterReminder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chapterReminderText;//章节提醒文本
    void OnEnable()
    {
        EventHandler.levelLoaded += OnLevelLoaded;
    }
    void OnDisable()
    {
        EventHandler.levelLoaded -= OnLevelLoaded;
    }

    void OnLevelLoaded()
    {
        chapterReminderText.text = $"Chapter{GameManager.Instance.getChapterNumber} Level{GameManager.Instance.getLevelNumber}";
    }
}
