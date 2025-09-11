using UnityEngine;
using UnityEngine.UI;

public class SlotItemUI : MonoBehaviour
{
    [SerializeField] private Image itemImage = null;
    [SerializeField] private GameObject selectedBorder = null;

    public void SetIcon(Sprite icon)
    {
        itemImage.sprite = icon;
        itemImage.color = icon == null ? new Color(0f, 0f, 0f, 0f) : Color.white;
    }

    public void ToggleSelected(bool status)
    {
        selectedBorder.SetActive(status);
    }
}
