using System;
using System.Collections;

using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private float lerpTime = 1f;

    private void Awake()
    {
        GameManager.Instance.LoadingManager.SetLoadingUI(this);
    }

    public void ToggleUI(bool status, Action onComplete = null)
    {
        StartCoroutine(LoadingCoroutine());
        IEnumerator LoadingCoroutine()
        {
            float timer = 0f;
            float startAlpha = canvasGroup.alpha;
            float targetAlpha = status ? 1f : 0f;

            while (timer < lerpTime)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / lerpTime);

                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = status;
            onComplete?.Invoke();
        }
    }
}
