using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
  [Header("General Settings")]
  [SerializeField] private float speed = 0f;
  [SerializeField] private PlayerItemDetection itemDetection = null;

  [Header("Weapon System")]
  [SerializeField] private WeaponHolder weaponHolder;

  [Header("Revive Settings")]
  [SerializeField] private float reviveTime = 10f;
  [SerializeField] private float reviveAmount = 50f;
  [SerializeField] private float reviveDetectRange = 2f;

  private PlayerInputController inputController = null;
  private CharacterController characterController = null;
  private PlayerInventory inventory = null;
  private PlayerHealth playerHealth = null;
  private PlayerUI playerUI = null;
  private PlayerSpawn playerSpawn = null;
  private Coroutine reviveCoroutine = null;
  private PlayerHealth targetToRevive = null;

  private Vector3 velocity = Vector3.zero;

  private Action onPause = null;

  private void Awake()
  {
    inputController = GetComponent<PlayerInputController>();
    characterController = GetComponent<CharacterController>();
    inventory = GetComponent<PlayerInventory>();
    playerHealth = GetComponent<PlayerHealth>();

    // weaponHolder puede estar en el mismo GameObject o como hijo
    if (weaponHolder == null)
      weaponHolder = GetComponentInChildren<WeaponHolder>();
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
    playerSpawn = FindFirstObjectByType<PlayerSpawn>();
  }

  private void Update()
  {
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
      ItemData itemData = itemEquipable.GetItem();
      inventory.EquipItem(itemEquipable.GetItem());
      playerUI.OnEquipItem(inventory.SelectedIndex, itemData);
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
    Debug.Log("HandleReviveInput: " + isPressed);
    if (playerHealth.IsDowned) return;

    if (isPressed)
    {
      targetToRevive = FindDownedTeammate();
      if (targetToRevive != null)
      {
        Debug.Log("Iniciando revivir a " + targetToRevive.name);
        reviveCoroutine = StartCoroutine(ReviveTeammateCoroutine());
      }
      else
      {
        Debug.Log("No hay compañeros para revivir cerca");
      }
    }
    else
    {
      // Si el botón se suelta, cancela el revivir
      if (reviveCoroutine != null)
      {
        StopCoroutine(reviveCoroutine);
        reviveCoroutine = null;
        // TODO: feedback visual o evento de cancelación
      }
    }
  }

  private PlayerHealth FindDownedTeammate()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, reviveDetectRange);

    foreach (var hitCollider in hitColliders)
    {
      PlayerHealth teammateHealth = hitCollider.GetComponent<PlayerHealth>();
      if (teammateHealth != null && teammateHealth != playerHealth && teammateHealth.IsDowned)
      {
        return teammateHealth;
      }
    }
    return null;
  }

  private IEnumerator ReviveTeammateCoroutine()
  {
    float timer = 0f;
    while (timer < reviveTime)
    {
      // TODO: barra de progreso en la UI o feedback visual arriba del cadaver
      Debug.Log("Reviviendo... " + timer + " / " + reviveTime);
      timer += Time.deltaTime;
      yield return null;
    }

    if (targetToRevive != null)
    {
      targetToRevive.Revive(reviveAmount);
      targetToRevive = null;
    }
  }
}
