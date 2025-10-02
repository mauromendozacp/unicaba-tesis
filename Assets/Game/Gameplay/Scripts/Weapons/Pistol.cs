using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Setup")]
    [SerializeField] private bool isDefault = true;        // Arma por defecto
    [SerializeField] private Transform muzzlePoint;
    
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolInitialSize = 16;

    private float nextFireTime;

    // Disparo simple. Munición ilimitada si es default.
    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        
        // Si NO es default, chequea munición normal
        if (!isDefault)
        {
            if (currentAmmo <= 0) return;
            currentAmmo--;
        }
        else
        {
            // Asegura que al menos tenga un valor visible > 0 para UI si ésta lo muestra
            if (currentAmmo <= 0) currentAmmo = 1;
        }

        nextFireTime = Time.time + fireRate;

        if (muzzlePoint == null || projectilePrefab == null) return;

        // Obtener (lazy) el pool cada vez: evita problemas de orden de creación
        var pool = ProjectilePoolManager.Instance?.GetPool(projectilePrefab, poolInitialSize);
        if (pool == null) return;

        var proj = pool.Get();
        proj.transform.position = muzzlePoint.position;
        proj.transform.rotation = muzzlePoint.rotation;
        proj.SetDirection(muzzlePoint.forward);
    }
}
