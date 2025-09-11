using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private SlotItemUI[] slots = null;

    private void OnChangeItemSelected(int index)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ToggleSelected(i == index);
        }
    }

    private void OnItemEquip(int index, ItemData item)
    {
        slots[index].SetIcon(item.Icon);
    }

    private void OnItemConsume(int index)
    {
        slots[index].SetIcon(null);
    }
}
