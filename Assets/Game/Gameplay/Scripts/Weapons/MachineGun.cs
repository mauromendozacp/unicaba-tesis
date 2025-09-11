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

        if (projectilePool != null && muzzlePoint != null)
        {
            Projectile proj = projectilePool.Get();
            proj.transform.position = muzzlePoint.position;
            proj.transform.rotation = muzzlePoint.rotation;
            proj.transform.SetParent(null);
            proj.SetDirection(muzzlePoint.forward);
            proj.SetPool(projectilePool);
        }
    }
}
