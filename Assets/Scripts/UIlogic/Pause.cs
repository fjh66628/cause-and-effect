using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Pause : MonoBehaviour
{
    [SerializeField] private Image pauseImage;//暂停图片
    [SerializeField] private Image resume;//继续按钮
    public void PauseGame()
    {
        if (GameManager.Instance.getGameState != GameState.Pause)
        {
            GameManager.Instance.SetGameState(GameState.Pause);
            pauseImage.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.Play);
            pauseImage.gameObject.SetActive(false);
        }
    }
    public void GameOver()
    {
        GameManager.Instance.SetGameState(GameState.Pause);
        resume.gameObject.SetActive(false);
    }
}
