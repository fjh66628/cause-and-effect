using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
[System.Serializable]
public class Buttons : MonoBehaviour
{
    public void StartGame()
    {
        string currentScene = "StartScene";
        string targetScene = "Chapter1Floor1";
        LoadManager.Instance.LoadLevel(currentScene, targetScene);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        LoadManager.Instance.LoadLevelManager();
        LoadingAnimator.Instance.SetLoading("选择场景");
    }

}
