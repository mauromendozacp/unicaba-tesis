using UnityEngine;

[CreateAssetMenu(menuName = "Items/WeaponData")]
public class WeaponData : ItemData
{
    [Header("Weapon Stats")]
    public float damage;
    public float fireRate;
    public int maxAmmo;
    public bool isDefault;

    public override void Use(GameObject user)
    {
        var holder = user.GetComponentInChildren<WeaponHolder>();
        if (holder != null && Prefab != null)
        {
            // Instanciar el arma desde el prefab y inicializarla con este SO
            var go = Instantiate(Prefab);
            var weapon = go.GetComponent<WeaponBase>();
            if (weapon != null)
            {
                weapon.Init(this);
                holder.EquipWeapon(weapon);
            }
            else
            {
                Debug.LogError($"[WeaponData] El prefab {Prefab.name} no tiene un WeaponBase.");
                Destroy(go);
            }
        }
    }
}