using UnityEngine;

[CreateAssetMenu(menuName = "Items/WeaponData")]
public class WeaponData : ItemData
{
    [Header("Weapon Stats")]
    public float damage;
    public float fireRate;
    public int maxAmmo;
    public bool isDefault;
    public GameObject weaponPrefab; // Prefab con WeaponBase

    public override void Use(GameObject user)
    {
        var holder = user.GetComponentInChildren<WeaponHolder>();
        if (holder != null && weaponPrefab != null)
        {
            // Instanciar el arma desde el prefab y inicializarla con este SO
            var go = Instantiate(weaponPrefab);
            var weapon = go.GetComponent<WeaponBase>();
            if (weapon != null)
            {
                weapon.Init(this);
                holder.EquipWeapon(weapon);
            }
            else
            {
                Debug.LogError($"[WeaponData] El prefab {weaponPrefab.name} no tiene un WeaponBase.");
                Destroy(go);
            }
        }
    }
}