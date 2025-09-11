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
}
