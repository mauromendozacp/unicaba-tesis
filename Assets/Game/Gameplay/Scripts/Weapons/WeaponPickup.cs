using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponBase weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        var weaponHolder = other.GetComponentInChildren<WeaponHolder>();
        if (weaponHolder != null && weaponPrefab != null)
        {
            weaponHolder.EquipWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}
