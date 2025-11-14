using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputGameplayController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private PlayerData defaultPlayerData = null;
    [SerializeField] private PlayerItemDetection itemDetection = null;
    [SerializeField] private SpriteMinimapIcon minimapSprite = null;

    [Header("Weapon System")]
    [SerializeField] private WeaponHolder weaponHolder;

    [Header("Visual Effects")]
    [SerializeField] private GameObject speedEffectPrefab;

    private ReviveController reviveController = null;
    private GameObject activeSpeedEffect = null;

    private PlayerInputGameplayController inputController = null;
    private CharacterController characterController = null;
    private PlayerInventory inventory = null;
    private PlayerHealth playerHealth = null;
    private PlayerUI playerUI = null;
    private PlayerAnimationController animationController = null;
    private PlayerItemCollector itemCollector = null;

    private PlayerData data = null;

    private float speed = 0f;
    private float baseSpeed = 0f;
    private Vector3 velocity = Vector3.zero;

    private Action onPause = null;
    private Camera mainCam = null;
    private Action onDeath = null;
    private Action onCollectKey = null;

    public PlayerHealth PlayerHealth => playerHealth;

    private void Awake()
    {
        inputController = GetComponent<PlayerInputGameplayController>();
        characterController = GetComponent<CharacterController>();
        inventory = GetComponent<PlayerInventory>();
        playerHealth = GetComponent<PlayerHealth>();
        reviveController = GetComponent<ReviveController>();
        animationController = GetComponentInChildren<PlayerAnimationController>();
        itemCollector = GetComponent<PlayerItemCollector>();

        if (weaponHolder == null)
            weaponHolder = GetComponentInChildren<WeaponHolder>();

        mainCam = Camera.main;
    }

    private void Start()
    {
        inventory.Init(inputController);

        inputController.onEquipItem += EquipItem;
        inputController.onUseItem += UseItem;
        inputController.onNextItem += ChangeSlot;
        inputController.onPreviousItem += ChangeSlot;
        inputController.onPause += onPause;

        playerUI?.ChangeSlot(inventory.SelectedIndex);

        playerHealth.OnUpdateLife += playerUI.OnUpdateLife;
        playerHealth.OnDeath += (player) => { animationController.ToggleDead(true); };
        playerHealth.OnDeath += (player) => { onDeath?.Invoke(); };
        playerHealth.OnRevived += (player) => { animationController.ToggleDead(false); };
        playerHealth.SetInitialData(data.MaxLife);

        inputController.onRevive += HandleReviveInput;

        Action collectorAction = onCollectKey;
        if (KeysManager.Instance != null)
        {
            collectorAction += KeysManager.Instance.AddKey;
        }

        itemCollector.Init(collectorAction);
    }

    public void Init(PlayerUI playerUI, PlayerData data, Sprite minimapIcon, Action onPause, Action onDeath, Action onCollectKey)
    {
        this.playerUI = playerUI;
        this.onPause = onPause;
        this.onDeath = onDeath;
        this.onCollectKey = onCollectKey;

        weaponHolder?.SetPlayerUI(playerUI);

        if (data == null)
            data = defaultPlayerData;

        this.data = data;
        GameObject playerPrefab = Instantiate(data.Prefab, transform);
        playerPrefab.transform.SetPositionAndRotation(data.PositionOffset, Quaternion.identity);

        this.playerUI.SetPlayerIcon(data.InventoryIcon);
        speed = data.Speed;
        baseSpeed = data.Speed;

        //minimapSprite.sprite = data.MinimapIcon;
        minimapSprite.SetSprite(minimapIcon);
    }

    private void Update()
    {
        if (reviveController.IsReviving) return;
        Move();
        HandleFireInput();
    }

    private void Move()
    {
        if (!characterController.enabled || !inputController.enabled) return;
        Vector2 moveInput = inputController.GetInputMove();

        Vector3 f = mainCam != null ? mainCam.transform.forward : transform.forward;
        Vector3 r = mainCam != null ? mainCam.transform.right : transform.right;
        f.y = 0f; r.y = 0f;
        f.Normalize(); r.Normalize();

        Vector3 move = r * moveInput.x + f * moveInput.y;

        characterController.Move(speed * Time.deltaTime * move);

        animationController?.UpdateMoveAnimation(move);

        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleFireInput()
    {
        if (inputController.GetInputFire())
        {
            weaponHolder?.CurrentWeapon?.Fire();
            if (weaponHolder?.CurrentWeapon != null)
            {
                playerUI?.OnUpdateAmmo(weaponHolder.CurrentWeapon.CurrentAmmo);
            }
        }
    }

    private void UseItem()
    {
        playerUI.OnUseItem(inventory.SelectedIndex);
    }

    private void EquipItem()
    {
        IEquipable itemEquipable = itemDetection.GetFirstItemDetection();
        if (itemEquipable != null)
        {
            if (itemEquipable.GetItem() is WeaponData)
            {
                WeaponData weaponData = itemEquipable.GetItem() as WeaponData;
                GameObject weaponGO = Instantiate(weaponData.Prefab, weaponHolder.transform);
                var weapon = weaponGO.GetComponent<WeaponBase>();
                weapon.Init(weaponData);
                weaponHolder.EquipWeapon(weapon);
            }
            else
            {
                ItemData itemData = itemEquipable.GetItem();
                int equipIndex = inventory.EquipItem(itemEquipable.GetItem());
                playerUI.OnEquipItem(equipIndex, itemData);
            }
            itemEquipable.Equip();
        }
    }

    private void ChangeSlot()
    {
        playerUI.ChangeSlot(inventory.SelectedIndex);
    }

    private void OnDestroy()
    {
        inputController.onRevive -= HandleReviveInput;
    }

    private void HandleReviveInput(bool isPressed)
    {
        if (playerHealth.IsDowned) return;

        if (isPressed)
        {
            reviveController.TryStartRevive();
        }
        else
        {
            reviveController.CancelRevive();
        }
    }

    public void ToggleInput(bool status)
    {
        if (status)
        {
            inputController.PlayerMap.Enable();
        }
        else
        {
            inputController.PlayerMap.Disable();
        }
    }

    public void ToggleGameplayInputs(bool status)
    {
        if (status)
        {
            inputController.PlayerMap.Enable();
        }
        else
        {
            inputController.PlayerMap.Disable();
        }
    }

    public void ModifySpeed(float multiplier)
    {
        speed = baseSpeed * multiplier;
    }

    public void RestoreSpeed()
    {
        speed = baseSpeed;
    }

    public void EnableSpeedEffect()
    {
        if (speedEffectPrefab != null && activeSpeedEffect == null)
        {
            activeSpeedEffect = Instantiate(speedEffectPrefab, transform);
            activeSpeedEffect.transform.localPosition = Vector3.zero;
        }
    }

    public void DisableSpeedEffect()
    {
        if (activeSpeedEffect != null)
        {
            Destroy(activeSpeedEffect);
            activeSpeedEffect = null;
        }
    }
}
