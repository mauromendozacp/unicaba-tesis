using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    [SerializeField] private WeaponBase defaultWeaponPrefab;
    [SerializeField] private Transform weaponParent;

    private WeaponBase currentWeapon;
    public IWeapon CurrentWeapon => currentWeapon;

    private void Start()
    {
        EquipDefaultWeapon();
    }

    public void EquipDefaultWeapon()
    {
        EquipWeapon(defaultWeaponPrefab);
    }

    public void EquipWeapon(WeaponBase weaponPrefab)
    {
        // Si el arma es del mismo tipo, solo suma munición
        if (currentWeapon != null && currentWeapon.GetType() == weaponPrefab.GetType() && !currentWeapon.IsDefault)
        {
            currentWeapon.AddAmmo(weaponPrefab.MaxAmmo);
            return;
        }

        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        if (weaponParent == null)
        {
            weaponParent = transform.Find("Hand");
            if (weaponParent == null)
            {
                Debug.LogError($"[WeaponHolder] No se asignó weaponParent en {name} y no se encontró un hijo 'Hand'.");
                return;
            }
        }

        currentWeapon = Instantiate(weaponPrefab, weaponParent.position, weaponParent.rotation, weaponParent);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.OnPickup();
    }

    public void DropWeapon()
    {
        if (currentWeapon != null && !currentWeapon.IsDefault)
        {
            Destroy(currentWeapon.gameObject);
            EquipDefaultWeapon();
        }
    }

    private void Update()
    {
        if (currentWeapon != null && currentWeapon.IsAmmoEmpty && !currentWeapon.IsDefault)
        {
            DropWeapon();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (weaponParent != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(weaponParent.position, 0.05f);
            UnityEditor.Handles.Label(weaponParent.position, "Weapon Parent");
        }
    }
#endif
}
