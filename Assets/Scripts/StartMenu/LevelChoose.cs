using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LevelChoose : MonoBehaviour
{
    [SerializeField] int ChapterNumber;
    [SerializeField] int levelNumber;
    public void LoadLevel()
    {
        LoadManager.Instance.LoadLevel(ChapterNumber, levelNumber);
    }
}
