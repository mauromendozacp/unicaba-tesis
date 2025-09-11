using UnityEngine;

public class MachineGunPickup : MonoBehaviour
{
    [SerializeField] private MachineGun machineGunPrefab;

    private void OnTriggerEnter(Collider other)
    {
        var weaponHolder = other.GetComponentInChildren<WeaponHolder>();
        if (weaponHolder != null && machineGunPrefab != null)
        {
            weaponHolder.EquipWeapon(machineGunPrefab);
            Destroy(gameObject);
        }
    }
}
