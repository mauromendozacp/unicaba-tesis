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
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }
        if (weaponParent == null)
        {
            // Intento de recuperación automática
            weaponParent = transform.Find("Hand");
            if (weaponParent == null)
            {
                Debug.LogError($"[WeaponHolder] No se asignó weaponParent en {name} y no se encontró un hijo 'Hand'.");
                return;
            }
        }

        // Instancia explícitamente en la posición/rotación de la mano
        currentWeapon = Instantiate(weaponPrefab, weaponParent.position, weaponParent.rotation, weaponParent);

        // Normaliza offset local (asegura que quede alineada a la mano)
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
