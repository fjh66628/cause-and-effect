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
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene("LevelManager");
    }
    public void LoadLevel(int Chapter, int Level)
    {
        StartCoroutine(LoadLevelCoroutine(Chapter, Level));
    }
    IEnumerator LoadLevelCoroutine(string sceneName, string targetName)
    {
        LoadingAnimator.Instance.SetLoading(targetName);
        yield return new WaitForSeconds(1.2f);

        // 1. 卸载原有场景 + 加载UI场景
        yield return SceneManager.LoadSceneAsync("UI");
        yield return null;

        // 2. 加载目标场景
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;

        // 3. 数据初始化（地图、卡牌）
        EventHandler.CallLevelDataReady();
        yield return null;

        // 4. 渲染（物品、相机、UI）
        EventHandler.CallLevelLoaded();

    }
    IEnumerator LoadLevelCoroutine(int Chapter, int Level)
    {
        LevelManagement levelInfo = LevelManager.Instance.GetLevelManagement(Chapter, Level);
        if (levelInfo == null)
        {
            Debug.LogError($"LoadLevelCoroutine: GetLevelManagement返回null, Chapter={Chapter}, Level={Level}");
            yield break;
        }
        int totalEndStep = levelInfo.getEndStepCount;
        int currentEndStep = GameManager.Instance.getEndStepCount;
        string sceneName = $"Chapter{Chapter}Floor{Level}";
        string displayText = $"Chapter{Chapter}Floor{Level} 终点{currentEndStep}/{totalEndStep}";
        LoadingAnimator.Instance.SetLoading(displayText);
        yield return new WaitForSeconds(1.2f);

        // 1. 卸载原有场景 + 加载UI场景
        yield return SceneManager.LoadSceneAsync("UI");
        yield return null;

        // 2. GameManager数据更新
        GameManager.Instance.SetLevelCount(Chapter, Level);

        // 3. 加载目标场景
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;

        // 4. 数据初始化（地图、卡牌）
        EventHandler.CallLevelDataReady();
        yield return null;

        // 5. 渲染（物品、相机、UI）
        EventHandler.CallLevelLoaded();
        EventHandler.CallUpdateCard();
        GameManager.Instance.SetGameState(GameState.Play);//游戏开始
    }

    public void LoadNextLevel(string currentSceneName, string nextSceneName)
    {
        StartCoroutine(LoadNextLevelCoroutine(currentSceneName, nextSceneName));
    }

    IEnumerator LoadNextLevelCoroutine(string currentSceneName, string nextSceneName)
    {
        GameManager.Instance.SetGameState(GameState.Pause);

        yield return new WaitForSeconds(0.5f);

        // 1. 卸载原有场景
        yield return SceneManager.UnloadSceneAsync(currentSceneName);
        Resources.UnloadUnusedAssets();
        yield return null;

        // 2. GameManager数据更新
        GameManager.Instance.ResetEndStepCount();
        GameManager.Instance.SetCardNumber();
        yield return null;

        // 3. 新场景加载
        yield return SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextSceneName));
        yield return null;

        // 4. 数据初始化（地图、卡牌）
        EventHandler.CallLevelDataReady();
        yield return null;

        // 5. 渲染（物品、相机、UI）
        EventHandler.CallLevelLoaded();
        EventHandler.CallUpdateCard();

        yield return new WaitForSeconds(2f);

        GameManager.Instance.SetGameState(GameState.Play);

    }

    public void LoadEndScene()
    {
        StartCoroutine(LoadEndSceneCoroutine());
    }

    IEnumerator LoadEndSceneCoroutine()
    {
        GameManager.Instance.SetGameState(GameState.Pause);

        yield return new WaitForSeconds(0.5f);

        yield return SceneManager.LoadSceneAsync("END", LoadSceneMode.Single);

        yield return new WaitForSeconds(0.5f);

    }

    public void ReloadCurrentLevel()
    {
        LoadingAnimator.Instance.SetLoading("重新加载关卡");
        StartCoroutine(ReloadCurrentLevelCoroutine());
    }

    IEnumerator ReloadCurrentLevelCoroutine()
    {
        GameManager.Instance.SetGameState(GameState.Pause);

        yield return new WaitForSeconds(0.5f);

        string currentSceneName = $"Chapter{GameManager.Instance.getChapterNumber}Level{GameManager.Instance.getLevelNumber}";

        // 1. 卸载原有场景
        yield return SceneManager.UnloadSceneAsync(currentSceneName);
        Resources.UnloadUnusedAssets();
        yield return null;

        // 2. GameManager数据更新
        GameManager.Instance.ResetGameStateForReload();
        yield return null;

        // 3. 新场景加载
        yield return SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));
        yield return null;

        // 4. 数据初始化（地图、卡牌）
        EventHandler.CallLevelDataReady();
        yield return null;

        // 5. 渲染（物品、相机、UI）
        EventHandler.CallLevelLoaded();
        EventHandler.CallUpdateCard();

        GameManager.Instance.SetGameState(GameState.Play);

        Debug.Log("关卡重新加载完成");
    }
}