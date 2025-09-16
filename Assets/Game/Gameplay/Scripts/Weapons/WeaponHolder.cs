using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private WeaponData defaultWeaponData;
    [SerializeField] private Transform weaponParent;

    [Header("UI")]
    [SerializeField] private PlayerUI playerUI;

    private WeaponBase currentWeapon;

    public IWeapon CurrentWeapon => currentWeapon;

    private void Start()
    {
        EquipDefaultWeapon();
    }

    public void SetPlayerUI(PlayerUI ui)
    {
        playerUI = ui;
        if (playerUI != null && currentWeapon != null)
        {
            playerUI.SetGunIcon(currentWeapon.Icon);
            playerUI.OnUpdateAmmo(currentWeapon.CurrentAmmo);
        }
    }

    public void EquipDefaultWeapon()
    {
        if (defaultWeaponData != null)
        {
            GameObject weaponGO = Instantiate(defaultWeaponData.weaponPrefab);
            var weapon = weaponGO.GetComponent<WeaponBase>();
            weapon.Init(defaultWeaponData);
            EquipWeapon(weapon);
        }
    }

    public void EquipWeapon(WeaponBase weaponInstance)
    {
        if (currentWeapon != null && currentWeapon.GetType() == weaponInstance.GetType() && !currentWeapon.IsDefault)
        {
            currentWeapon.AddAmmo(weaponInstance.MaxAmmo);
            Destroy(weaponInstance.gameObject);
            playerUI?.OnUpdateAmmo(currentWeapon.CurrentAmmo);
            playerUI?.SetGunIcon(currentWeapon.Icon);
            return;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = weaponInstance;
        currentWeapon.transform.SetParent(weaponParent != null ? weaponParent : transform, false);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.OnPickup();
        playerUI?.SetGunIcon(currentWeapon.Icon);
        playerUI?.OnUpdateAmmo(currentWeapon.CurrentAmmo);
    }

    public void DropWeapon()
    {
        if (currentWeapon != null && !currentWeapon.IsDefault)
        {
            Destroy(currentWeapon.gameObject);
            EquipDefaultWeapon();
            playerUI?.OnUpdateAmmo(currentWeapon != null ? currentWeapon.CurrentAmmo : 0);
        }
    }

    private void Update()
    {
        if (currentWeapon != null && currentWeapon.IsAmmoEmpty && !currentWeapon.IsDefault)
        {
            DropWeapon();
        }
    }
}
