using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Projectile")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolInitialSize = 32;
    [SerializeField] private ProjectileObjectPool projectilePool;
    [SerializeField] private Transform muzzlePoint;

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletsPerShot = 8;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private float reloadTime = 1.5f;
    [Header("Audio")]
    [SerializeField] private AudioEvent pumpAudioEvent; // sonido de pump/recarga corto

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
        if (projectilePool == null || muzzlePoint == null)
            if (projectilePool == null || muzzlePoint == null) return;

        currentAmmo--;

        for (int i = 0; i < pelletsPerShot; i++)
        {
            var proj = projectilePool.Get();
                if (proj == null) continue;
            
            proj.transform.position = muzzlePoint.position;
            proj.transform.rotation = muzzlePoint.rotation;

            Vector3 spreadDirection = CalculateSpread(muzzlePoint.forward);
            proj.SetDirection(spreadDirection);
        }

        PlayFireAudio(muzzlePoint.position);

        if (pumpAudioEvent != null && pumpAudioEvent.Clip != null)
        {
            CancelInvoke(nameof(PlayPumpSound));
            float pumpLength = pumpAudioEvent.Clip.length;
                float startAfter = reloadTime - pumpLength;
            if (startAfter < 0.05f) startAfter = 0.05f;
            Invoke(nameof(PlayPumpSound), startAfter);
        }

        nextFireTime = Time.time + reloadTime;
    }

    private void PlayPumpSound()
    {
        PlayAudioEvent(pumpAudioEvent, muzzlePoint != null ? muzzlePoint.position : transform.position);
    }

    private Vector3 CalculateSpread(Vector3 forward)
    {
        float randomX = Random.Range(-spreadAngle, spreadAngle);
        float randomY = Random.Range(-spreadAngle, spreadAngle);

        Quaternion spread = Quaternion.Euler(randomX, randomY, 0f);
        return spread * forward;
    }

    public bool IsReloading() => Time.time < nextFireTime;
}