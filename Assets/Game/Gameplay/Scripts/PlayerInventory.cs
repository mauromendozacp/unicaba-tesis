using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private ItemData[] slots = new ItemData[3];
    private int selectedIndex = 0;
    private HashSet<string> keys = new HashSet<string>();

    public int SelectedIndex => selectedIndex;

    public void Init(PlayerInputController inputController)
    {
        inputController.onUseItem += UseCurrentItem;
        inputController.onPreviousItem += SelectPreviousSlot;
        inputController.onNextItem += SelectNextSlot;
    }

    public void EquipItem(ItemData item)
    {
        int emptyItemIndex = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                emptyItemIndex = i;
                break;
            }
        }

        if (emptyItemIndex >= 0)
        {
            slots[emptyItemIndex] = item;
        }
        else
        {
            //falta implementar drop item
            slots[selectedIndex] = item;
        }
    }

    private void UseCurrentItem()
    {
        ItemData item = slots[selectedIndex];
        if (item != null)
        {
            item.Use(gameObject);
            slots[selectedIndex] = null;
        }
    }

    private void SelectPreviousSlot()
    {
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = slots.Length - 1;
        }
    }

    private void SelectNextSlot()
    {
        selectedIndex = (selectedIndex + 1) % slots.Length;
    }

    public ItemData GetItem(int slot)
    {
        return slots[slot];
    }

    public void AddKey(string doorId)
    {
        if (!string.IsNullOrEmpty(doorId))
        {
            keys.Add(doorId);
        }
    }

    public bool HasKey(string doorId)
    {
        return keys.Contains(doorId);
    }

    public void RemoveKey(string doorId)
    {
        if (keys.Contains(doorId))
        {
            keys.Remove(doorId);
        }
    }
}
