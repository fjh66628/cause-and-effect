using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Pause pause;//暂停
    public void QuitGameButton()
    {
        Application.Quit();
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
