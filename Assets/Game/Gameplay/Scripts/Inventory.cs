using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private ItemData[] slots = null;

    private int selectedIndex = 0;

    public void Init()
    {

    }

    public void AddItem(ItemData item, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex] = item;
        }
    }

    private void UseCurrentItem()
    {
        ItemData item = slots[selectedIndex];
        if (item != null)
        {
            item.Use(gameObject);
        }
    }

    private void SelectNextSlot()
    {
        selectedIndex = (selectedIndex + 1) % slots.Length;
    }

    private void SelectPreviousSlot()
    {
        selectedIndex--;
        if (selectedIndex < 0)
        {
            selectedIndex = slots.Length - 1;
        }
    }

    public ItemData GetItem(int slot)
    {
        return slots[slot];
    }
}
