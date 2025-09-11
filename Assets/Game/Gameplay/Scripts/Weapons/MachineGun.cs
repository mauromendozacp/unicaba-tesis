using UnityEngine;

public class MachineGun : WeaponBase
{
    [Header("Projectile Settings")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform muzzlePoint;

    private float nextFireTime = 0f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        if (currentAmmo <= 0) return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        Debug.Log("MachineGun Fire! Ammo left: " + currentAmmo);

        if (projectilePool != null && muzzlePoint != null)
        {
            Projectile proj = projectilePool.Get();
            proj.transform.position = muzzlePoint.position;
            proj.transform.rotation = muzzlePoint.rotation;
            proj.SetDirection(muzzlePoint.forward);
            proj.SetPool(projectilePool);
        }
        else
        {
            Debug.LogWarning($"[MachineGun] Falta asignar projectilePool o muzzlePoint en {name}");
        }
    }
}
