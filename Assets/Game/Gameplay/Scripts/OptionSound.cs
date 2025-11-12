using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionSound : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private Button btn = null;
    [SerializeField] private Sprite enableSprite = null;
    [SerializeField] private Sprite disableSprite = null;

    private bool currentStatus = false;

    public void Init(bool initialStatus, float initialValue, Action<float> onUpdateValue, Action<bool> onUpdateBtn)
    {
        slider.onValueChanged.AddListener(onUpdateValue.Invoke);
        btn.onClick.AddListener(() =>
        {
            currentStatus = !currentStatus;
            btn.image.sprite = currentStatus ? enableSprite : disableSprite;
            onUpdateBtn?.Invoke(currentStatus);
        });

        currentStatus = initialStatus;
        btn.image.sprite = currentStatus ? enableSprite : disableSprite;
        slider.value = initialValue;
    }
}
