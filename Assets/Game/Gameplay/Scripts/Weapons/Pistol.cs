using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Projectile Settings")]
    [SerializeField] private Projectile projectilePrefab;
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
        if (projectilePrefab != null && muzzlePoint != null)
        {
            Projectile proj = Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            proj.SetDirection(muzzlePoint.forward);
        }
        else
        {
            Debug.LogWarning($"[Pistol] Falta asignar projectilePrefab o muzzlePoint en {name}");
        }
    }

    public override bool IsAmmoEmpty => false; // Nunca se queda sin munición
}
