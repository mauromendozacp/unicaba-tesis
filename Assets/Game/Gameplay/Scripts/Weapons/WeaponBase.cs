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

    [Header("Audio")]
    [SerializeField] private AudioEvent fireAudioEvent;
    private AudioEvent lastPlayedAudioEvent = null;
    private float lastPlayedAudioTime = 0f;
    [SerializeField] private float audioDebounceWindow = 0.15f; 
    [SerializeField] private bool bypassAudioDebounce = false; 

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

    public virtual bool HasInfiniteAmmo => false;

    protected void PlayFireAudio(Vector3 position)
    {
        if (fireAudioEvent == null || fireAudioEvent.Clip == null) return;

        if (!bypassAudioDebounce)
        {
            if (lastPlayedAudioEvent == fireAudioEvent && Time.time - lastPlayedAudioTime < audioDebounceWindow)
                return;
        }
        bool played = false;
        try
        {
            if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
            {
                GameManager.Instance.AudioManager.PlayAudio(fireAudioEvent, position);
                played = true;
            }
        }
        catch {}

        if (!played)
            AudioSource.PlayClipAtPoint(fireAudioEvent.Clip, position, fireAudioEvent.Volume);

        lastPlayedAudioEvent = fireAudioEvent;
        lastPlayedAudioTime = Time.time;
    }

    protected void PlayAudioEvent(AudioEvent audioEvent, Vector3 position)
    {
        if (audioEvent == null || audioEvent.Clip == null) return;
        try
        {
            if (GameManager.Instance != null && GameManager.Instance.AudioManager != null)
            {
                GameManager.Instance.AudioManager.PlayAudio(audioEvent, position);
                return;
            }
        }
        catch { }

        AudioSource.PlayClipAtPoint(audioEvent.Clip, position, audioEvent.Volume);
    }
}

