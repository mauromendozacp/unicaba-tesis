using UnityEngine;

public class RangedWeapon : WeaponBase
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;

    public override void Fire()
    {
        if (IsAmmoEmpty) return;
        if (Time.time < lastFireTime + fireRate) return;

        lastFireTime = Time.time;
        currentAmmo--;

        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
    }
}
