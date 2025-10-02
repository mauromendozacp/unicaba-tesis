using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Slots de ítems equipables")]
    [SerializeField] private SlotItemUI[] slots = null;

    [Header("Panel de llaves (opcional)")]
    [SerializeField] private Transform keysPanel = null;
    [SerializeField] private GameObject keyIconPrefab = null;

    private Dictionary<string, GameObject> keyIcons = new Dictionary<string, GameObject>();

    public void OnChangeItemSelected(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ToggleSelected(i == index);
        }
    }

    public void OnItemEquip(int index, ItemData item)
    {
        if (index >= 0 && index < slots.Length)
        {
            slots[index].SetIcon(item != null ? item.Icon : null);
        }
    }

    public void OnItemConsume(int index)
    {
        if (index >= 0 && index < slots.Length)
        {
            slots[index].SetIcon(null);
        }
    }

    public void OnAddKey(string keyName, Sprite icon = null)
    {
        if (keysPanel == null || keyIconPrefab == null)
        {
            return;
        }

        if (keyIcons.ContainsKey(keyName))
        {
            return;
        }

        GameObject newKey = Instantiate(keyIconPrefab, keysPanel);
        if (icon != null)
        {
            Image image = newKey.GetComponentInChildren<Image>();
            if (image != null)
                image.sprite = icon;
        }

        TMP_Text text = newKey.GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = keyName;

        keyIcons[keyName] = newKey;
    }
}
