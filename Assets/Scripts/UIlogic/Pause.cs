using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Pause : MonoBehaviour
{
    [SerializeField] private Image pauseImage;//暂停图片
    bool isPaused = false;//是否暂停
    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;//设置暂停为true
            GameManager.Instance.SetGameState(GameState.Pause);
            pauseImage.gameObject.SetActive(true);
        }
        else
        {
            isPaused = false;//设置暂停为false
            GameManager.Instance.SetGameState(GameState.Play);
            pauseImage.gameObject.SetActive(false);
        }
    }

}
