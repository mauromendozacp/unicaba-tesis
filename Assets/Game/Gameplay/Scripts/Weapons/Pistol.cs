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

    public override void Fire()
    {
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
