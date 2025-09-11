using UnityEngine;

public interface IWeapon
{
    void Fire();
    void Reload(int ammo);
    void OnPickup();
    void OnDrop();
    bool IsAmmoEmpty { get; }
    int CurrentAmmo { get; }
    int MaxAmmo { get; }
    float Damage { get; }
    float FireRate { get; }
    bool IsDefault { get; }
}
