using System;
using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
  [Header("General Settings")]
  [SerializeField] private float speed = 0f;
  [SerializeField] private PlayerItemDetection itemDetection = null;

  [Header("Weapon System")]
  [SerializeField] private WeaponHolder weaponHolder;

  private ReviveController reviveController = null;

  private PlayerInputController inputController = null;
  private CharacterController characterController = null;
  private PlayerInventory inventory = null;
  private PlayerHealth playerHealth = null;
  private PlayerUI playerUI = null;
  //private PlayerSpawn playerSpawn = null;
  //private Coroutine reviveCoroutine = null;
  //private PlayerHealth targetToRevive = null;
  //bool isReviving = false;

  private Vector3 velocity = Vector3.zero;

  private Action onPause = null;
  //private InputAction fireAction;

  private void Awake()
  {
    inputController = GetComponent<PlayerInputController>();
    characterController = GetComponent<CharacterController>();
    inventory = GetComponent<PlayerInventory>();
    playerHealth = GetComponent<PlayerHealth>();
    reviveController = GetComponent<ReviveController>();

    // weaponHolder puede estar en el mismo GameObject o como hijo
    if (weaponHolder == null)
      weaponHolder = GetComponentInChildren<WeaponHolder>();

    //if (reviveEffectSphere != null) reviveEffectSphere.SetActive(false);
    //if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(false);
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

    inputController.onRevive += HandleReviveInput;
    //playerSpawn = FindFirstObjectByType<PlayerSpawn>();
  }

  private void Update()
  {
    if (reviveController.IsReviving) return;
    Move();
    HandleFireInput();
  }

  public void Init(PlayerUI playerUI, PlayerData data, Action onPause)
  {
    this.playerUI = playerUI;
    this.onPause = onPause;

    weaponHolder?.SetPlayerUI(playerUI);
  }

  private void Move()
  {
    if (!characterController.enabled || !inputController.enabled) return;
    Vector2 moveInput = inputController.GetInputMove();

    Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
    characterController.Move(speed * Time.deltaTime * move);

    if (characterController.isGrounded && velocity.y < 0)
    {
      velocity.y = -2f;
    }

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
        inventory.EquipItem(itemEquipable.GetItem());
        playerUI.OnEquipItem(inventory.SelectedIndex, itemData);
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
      // Si el botón se suelta, cancela el revivir
      reviveController.CancelRevive();
    }
  }

}
