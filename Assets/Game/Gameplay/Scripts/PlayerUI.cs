using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image playerImage = null;

    [SerializeField] private Image gunImage = null;
    [SerializeField] private GameObject limitedAmmo = null;
    [SerializeField] private GameObject unlimitedAmmo = null;
    [SerializeField] private TMP_Text currentAmmo = null;

    [SerializeField] private PlayerInventoryUI inventory = null;

    public void SetPlayerIcon(Sprite icon)
    {
        playerImage.sprite = icon;
    }

    public void OnChangeGun()
    {

    }

    public void OnUpdateAmmo(int ammo)
    {
        currentAmmo.text = ammo.ToString(); 
    }

    public void ChangeSlot(int index)
    {
        inventory.OnChangeItemSelected(index);
    }

    public void OnUseItem(int index)
    {
        inventory.OnItemConsume(index);
    }

    public void OnEquipItem(int index, ItemData item)
    {
        inventory.OnItemEquip(index, item);
    }
}
