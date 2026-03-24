using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
[System.Serializable]
public class Buttons : MonoBehaviour
{
    [SerializeField] private Canvas canvasGroup;
    public void StartGame()
    {
        string currentScene = "StartScene";
        string targetScene = "Chapter1Floor1";
        canvasGroup.gameObject.SetActive(false);
        LoadManager.Instance.LoadLevel(currentScene, targetScene);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        canvasGroup.gameObject.SetActive(false);
        LoadManager.Instance.LoadLevelManager();
    }

}
