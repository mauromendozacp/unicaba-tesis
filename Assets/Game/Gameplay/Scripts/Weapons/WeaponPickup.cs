using UnityEngine;

using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    private WeaponHolder playerInRange;
    private PlayerInput playerInput;

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = other.GetComponentInChildren<WeaponHolder>();
        playerInput = other.GetComponent<PlayerInput>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInRange != null && other.GetComponentInChildren<WeaponHolder>() == playerInRange)
        {
            playerInRange = null;
            playerInput = null;
        }
    }

    private void Update()
    {
        if (playerInRange != null && playerInput != null)
        {
            var interactAction = playerInput.actions["Interact"];
            if (interactAction != null && interactAction.WasPressedThisFrame())
            {
                GameObject weaponGO = Instantiate(weaponData.Prefab, playerInRange.transform);
                var weapon = weaponGO.GetComponent<WeaponBase>();
                weapon.Init(weaponData);
                playerInRange.EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
