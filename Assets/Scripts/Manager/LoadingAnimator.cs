using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class LoadingAnimator : SingletonMono<LoadingAnimator>
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI loadingTextMeshPro;
    [SerializeField] private Image loadingPanel;
    public void SetLoading(string animationText)
    {
        loadingPanel.gameObject.SetActive(true);
        StartCoroutine(LoadingAnimation(animationText));
    }
    IEnumerator LoadingAnimation(string text)
    {
        yield return new WaitForSeconds(0.8f);
        animator.SetTrigger("Loading");
        animator.ResetTrigger("LoadingEnd");
        loadingTextMeshPro.text = text;
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("LoadingEnd");
        animator.ResetTrigger("Loading");
        loadingTextMeshPro.text = "";
        loadingPanel.gameObject.SetActive(false);
    }
}
