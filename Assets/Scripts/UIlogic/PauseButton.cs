using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Pause pause;//鏆傚仠
    public void QuitGameButton()
    {
        StartCoroutine(ResumeGameButtonCoroutine());
        //Application.Quit();
    }
    IEnumerator ResumeGameButtonCoroutine()
    {
        LoadingAnimator.Instance.SetLoading("\u8fd4\u56de\u4e3b\u83dc\u5355");
        yield return new WaitForSeconds(1.2f);
        yield return SceneManager.LoadSceneAsync("StartScene");
        yield return new WaitForSeconds(0.6f);
        CameraManager.Instance.ReSetCameraPosition();//閲嶇疆鐩告満浣嶇疆
    }
    public void ResumeGameButton()
    {
        LoadManager.Instance.ReloadCurrentLevel();
        pause.PauseGame();//鏆傚仠娓告垙
    }
    public void RestartGameButton()//閲嶆柊寮€濮嬫父鎴忔寜閽?
    {
        LoadManager.Instance.ReloadCurrentLevel();
        pause.GameOver();//鏆傚仠娓告垙
    }
}

