using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : SingletonMono<LoadManager>
{
    const string UISceneName = "UI";
    const string EndSceneName = "END";
    const float LoadSceneDelay = 0.8f;

    bool isLoading = false;

    public void LoadLevel(string currentSceneName, string targetName)
    {
        if (TryGetLevelNumbers(targetName, out int chapter, out int level))
        {
            StartCoroutine(LoadLevelCoroutine(chapter, level, currentSceneName, true));
            return;
        }

        StartCoroutine(LoadSceneByNameCoroutine(currentSceneName, targetName));
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

    public void LoadLevel(int chapter, int level)
    {
        StartCoroutine(LoadLevelCoroutine(chapter, level, null, true));
    }

    public void LoadNextLevel()
    {
        int currentChapter = GameManager.Instance.getChapterNumber;
        int currentLevel = GameManager.Instance.getLevelNumber;
        string currentSceneName = GetLevelSceneName(currentChapter, currentLevel);

        int nextChapter = currentChapter;
        int nextLevel = currentLevel + 1;

        if (nextLevel > LevelManager.Instance.GetLevelCount(nextChapter))
        {
            nextLevel = 1;
            nextChapter++;
        }

        if (!LevelManager.Instance.HasLevel(nextChapter, nextLevel))
        {
            LoadingAnimator.Instance.SetLoading("\u6e38\u620f\u7ed3\u675f");
            StartCoroutine(LoadEndSceneCoroutine());
            return;
        }

        StartCoroutine(LoadLevelCoroutine(nextChapter, nextLevel, currentSceneName, false));
    }

    public void ReloadCurrentLevel()
    {
        int chapter = GameManager.Instance.getChapterNumber;
        int level = GameManager.Instance.getLevelNumber;
        string currentSceneName = GetLevelSceneName(chapter, level);

        StartCoroutine(LoadLevelCoroutine(chapter, level, currentSceneName, false));
    }

    public void LoadEndScene()
    {
        LoadingAnimator.Instance.SetLoading("\u6e38\u620f\u7ed3\u675f");
        StartCoroutine(LoadEndSceneCoroutine());
    }

    IEnumerator LoadLevelCoroutine(int chapter, int level, string currentSceneName, bool loadUIScene)
    {
        if (isLoading)
        {
            yield break;
        }

        isLoading = true;

        string targetName = GetLevelSceneName(chapter, level);
        bool hasLevelData = LevelManager.Instance.HasLevel(chapter, level);
        LoadingAnimator.Instance.SetLoading(hasLevelData ? BuildLoadingText(chapter, level) : targetName);
        yield return new WaitForSeconds(LoadSceneDelay);

        if (loadUIScene)
        {
            yield return SceneManager.LoadSceneAsync(UISceneName);
        }

        GameManager.Instance.SetGameState(GameState.Pause);
        if (!LevelManager.Instance.HasLevel(chapter, level))
        {
            Debug.LogError($"Level data not found: Chapter{chapter}Level{level}");
            isLoading = false;
            yield break;
        }

        if (!hasLevelData)
        {
            LoadingAnimator.Instance.SetLoading(BuildLoadingText(chapter, level));
            yield return new WaitForSeconds(LoadSceneDelay);
        }

        if (!string.IsNullOrEmpty(currentSceneName) && SceneManager.GetSceneByName(currentSceneName).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(currentSceneName);
            Resources.UnloadUnusedAssets();
            yield return null;
        }

        GameManager.Instance.SetLevelCount(chapter, level);
        yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetName));

        EventHandler.CallLevelLoaded();
        GameManager.Instance.ResetGameStateForLevelLoad();
        EventHandler.CallUpdateCard();
        GameManager.Instance.SetGameState(GameState.Play);

        isLoading = false;
    }

    IEnumerator LoadSceneByNameCoroutine(string currentSceneName, string targetName)
    {
        if (isLoading)
        {
            yield break;
        }

        isLoading = true;
        GameManager.Instance.SetGameState(GameState.Pause);
        LoadingAnimator.Instance.SetLoading(targetName);
        yield return new WaitForSeconds(1.2f);

        if (!string.IsNullOrEmpty(currentSceneName) && SceneManager.GetSceneByName(currentSceneName).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(currentSceneName);
        }

        yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);
        isLoading = false;
    }

    IEnumerator LoadEndSceneCoroutine()
    {
        if (isLoading)
        {
            yield break;
        }

        isLoading = true;
        GameManager.Instance.SetGameState(GameState.Pause);

        yield return new WaitForSeconds(0.5f);
        yield return SceneManager.LoadSceneAsync(EndSceneName, LoadSceneMode.Single);
        yield return new WaitForSeconds(0.5f);

        isLoading = false;
    }

    string BuildLoadingText(int chapter, int level)
    {
        int endStepCount = LevelManager.Instance.GetLevelManagement(chapter, level).getEndStepCount;
        return $"Chapter{chapter}Level{level}\n\u9700\u8981\u8d70\u8fc7{endStepCount}\u6b21\u7ec8\u70b9";
    }

    string GetLevelSceneName(int chapter, int level)
    {
        return $"Chapter{chapter}Floor{level}";
    }

    bool TryGetLevelNumbers(string sceneName, out int chapter, out int level)
    {
        chapter = 0;
        level = 0;

        if (string.IsNullOrEmpty(sceneName) || !sceneName.StartsWith("Chapter") || !sceneName.Contains("Floor"))
        {
            return false;
        }

        string[] parts = sceneName.Replace("Chapter", "").Split("Floor");
        return parts.Length == 2 && int.TryParse(parts[0], out chapter) && int.TryParse(parts[1], out level);
    }
}

