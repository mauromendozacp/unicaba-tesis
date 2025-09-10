using UnityEngine;

public class MachineGun : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        isDefault = false;
    }

    public override void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log("MachineGun Fire! Ammo left: " + currentAmmo);
            // Aquí iría la lógica de instanciar el proyectil, efectos, etc.
        }
        else
        {
            Debug.Log("No ammo left!");
        }
    }
}
