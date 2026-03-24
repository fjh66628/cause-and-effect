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
        SceneManager.LoadScene("LevelManager");
    }
    IEnumerator LoadLevelCoroutine(string sceneName, string targetName)
    {
        LoadingAnimator.Instance.SetLoading(targetName);
        yield return new WaitForSeconds(1f);
        yield return SceneManager.LoadSceneAsync("UI");
        yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        EventHandler.CallLevelLoaded();

    }
}
