using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private ItemData[] slots = null;
    private int selectedIndex = 0;

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
            }
        }

        if (emptyItemIndex >= 0)
        {
            slots[emptyItemIndex] = item;
        }
        else
        {
            slots[selectedIndex] = item;
            //falta implementar drop item
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
}
