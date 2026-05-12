using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Pause : MonoBehaviour
{
    [SerializeField] private Image pauseImage;//暂停图片

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

}
