using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Weapon Settings")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float fireRate = 0.5f;
    [SerializeField] protected int maxAmmo = 10;
    [SerializeField] protected bool isDefault = false;

    [SerializeField] protected int currentAmmo;
    protected float lastFireTime;

    public virtual float Damage => damage;
    public virtual float FireRate => fireRate;
    public virtual int MaxAmmo => maxAmmo;
    public virtual int CurrentAmmo => currentAmmo;
    public virtual bool IsAmmoEmpty => maxAmmo > 0 && currentAmmo <= 0;
    public virtual bool IsDefault => isDefault;

    protected virtual void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public abstract void Fire();

    public virtual void Reload(int ammo)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + ammo, 0, maxAmmo);
    }

    public virtual void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
    }

    public virtual void OnPickup() { }
    public virtual void OnDrop() { }
}
