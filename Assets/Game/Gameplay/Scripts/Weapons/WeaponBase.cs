using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    protected float damage;
    protected float fireRate;
    protected int maxAmmo;
    protected bool isDefault;
    protected int currentAmmo;
    protected float lastFireTime;

    protected WeaponData weaponData;

    public virtual float Damage => damage;
    public virtual float FireRate => fireRate;
    public virtual int MaxAmmo => maxAmmo;
    public virtual int CurrentAmmo => currentAmmo;
    public virtual bool IsAmmoEmpty => maxAmmo > 0 && currentAmmo <= 0;
    public virtual bool IsDefault => isDefault;
    public virtual Sprite Icon => weaponData != null ? weaponData.Icon : null;

    public virtual void Init(WeaponData data)
    {
        weaponData = data;
        damage = data.damage;
        fireRate = data.fireRate;
        maxAmmo = data.maxAmmo;
        currentAmmo = data.maxAmmo;
        isDefault = data.isDefault;
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

    // NUEVO: por defecto no es infinita
    public virtual bool HasInfiniteAmmo => false;
}

