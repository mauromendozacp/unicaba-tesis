using UnityEngine;

public class MachineGun : WeaponBase
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolInitialSize = 24;
    [SerializeField] private ProjectileObjectPool projectilePool;
    [SerializeField] private Transform muzzlePoint;

    private float nextFireTime = 0f;

    private void Awake()
    {
        if (projectilePool == null && projectilePrefab != null)
            projectilePool = ProjectilePoolManager.Instance?.GetPool(projectilePrefab, poolInitialSize);
    }

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        if (currentAmmo <= 0) return;
        if (projectilePool == null || muzzlePoint == null) return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        var proj = projectilePool.Get();
        proj.transform.position = muzzlePoint.position;
        proj.transform.rotation = muzzlePoint.rotation;
        proj.SetDirection(muzzlePoint.forward);
    }
}
