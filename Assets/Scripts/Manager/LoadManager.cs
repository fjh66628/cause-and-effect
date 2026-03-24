using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : SingletonMono<LoadManager>
{
    public void LoadLevel(string sceneName, string targetName)
    {
        StartCoroutine(LoadLevelCoroutine(sceneName, targetName));
    }
    public void LoadLevelManager()
    {
        StartCoroutine(LoadLevelManagerCoroutine());
    }
    IEnumerator LoadLevelManagerCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("LevelManager");
    }
    public void LoadLevel(int Chapter, int Level)
    {
        StartCoroutine(LoadLevelCoroutine(Chapter, Level));
    }
    IEnumerator LoadLevelCoroutine(string sceneName, string targetName)
    {
        LoadingAnimator.Instance.SetLoading(targetName);
        yield return new WaitForSeconds(1f);
        yield return SceneManager.LoadSceneAsync("UI");
        yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        EventHandler.CallLevelLoaded();

    }
    IEnumerator LoadLevelCoroutine(int Chapter, int Level)
    {
        string targetName = $"Chapter{Chapter}Floor{Level}";
        LoadingAnimator.Instance.SetLoading(targetName);
        yield return new WaitForSeconds(1f);
        yield return SceneManager.LoadSceneAsync("UI");
        GameManager.Instance.SetLevelCount(Chapter, Level);
        yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        EventHandler.CallLevelLoaded();
    }
}
