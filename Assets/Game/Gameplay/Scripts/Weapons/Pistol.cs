using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Projectile Settings")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform muzzlePoint;

    protected override void Awake()
    {
        base.Awake();
        isDefault = true;
        maxAmmo = -1; // -1 para munición infinita
        currentAmmo = -1;
    }

    private float nextFireTime = 0f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        Debug.Log("Pistol Fire!");
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
            Debug.LogWarning($"[Pistol] Falta asignar projectilePool o muzzlePoint en {name}");
        }
    }

    public override bool IsAmmoEmpty => false; // Nunca se queda sin munición
}
