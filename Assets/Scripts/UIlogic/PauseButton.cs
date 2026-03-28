using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Pause pause;//暂停
    public void QuitGameButton()
    {
        StartCoroutine(ResumeGameButtonCoroutine());
        //Application.Quit();
    }
    public IEnumerator ResumeGameButtonCoroutine()
    {
        LoadingAnimator.Instance.SetLoading("返回主菜单");
        yield return new WaitForSeconds(1.2f);
        yield return SceneManager.LoadSceneAsync("StartScene");
        yield return new WaitForSeconds(0.6f);
        CameraManager.Instance.ReSetCameraPosition();//重置相机位置
    }
    public void ResumeGameButton()
    {
        GameManager.Instance.ReloadCurrentLevel();
        pause.PauseGame();//暂停游戏
    }
    public void RestartGameButton()//重新开始游戏按钮
    {
        GameManager.Instance.ReloadCurrentLevel();
        pause.GameOver();//暂停游戏
    }
}
