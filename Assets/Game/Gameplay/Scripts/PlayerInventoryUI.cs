using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private SlotItemUI[] slots = null;

    public void OnChangeItemSelected(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ToggleSelected(i == index);
        }
    }

    public void OnItemEquip(int index, ItemData item)
    {
        slots[index].SetIcon(item.Icon);
    }

    public void OnItemConsume(int index)
    {
        slots[index].SetIcon(null);
    }
}
