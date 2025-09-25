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

  [Header("Revive Settings")]
  [SerializeField] private float reviveTime = 10f;
  [SerializeField] private float reviveAmount = 50f;
  [SerializeField] private float reviveDetectRange = 2f;

  [Header("Revive Feedback")]
  [SerializeField] private GameObject reviveEffectSphere;
  [SerializeField] private TextMeshProUGUI reviveTimerText;


  private PlayerInputController inputController = null;
  private CharacterController characterController = null;
  private PlayerInventory inventory = null;
  private PlayerHealth playerHealth = null;
  private PlayerUI playerUI = null;
  private PlayerSpawn playerSpawn = null;
  private Coroutine reviveCoroutine = null;
  private PlayerHealth targetToRevive = null;
  bool isReviving = false;

  private Vector3 velocity = Vector3.zero;

  private Action onPause = null;
  //private InputAction fireAction;

  private void Awake()
  {
    inputController = GetComponent<PlayerInputController>();
    characterController = GetComponent<CharacterController>();
    inventory = GetComponent<PlayerInventory>();
    playerHealth = GetComponent<PlayerHealth>();

    // weaponHolder puede estar en el mismo GameObject o como hijo
    if (weaponHolder == null)
      weaponHolder = GetComponentInChildren<WeaponHolder>();

    if (reviveEffectSphere != null) reviveEffectSphere.SetActive(false);
    if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(false);
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
    if (isReviving) return;
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
    if (playerHealth.IsDowned) return;

    if (isPressed)
    {
      if (reviveCoroutine == null)
      {
        targetToRevive = FindDownedTeammate();
        if (targetToRevive != null)
        {
          Debug.Log("Iniciando revivir a " + targetToRevive.name);
          reviveCoroutine = StartCoroutine(ReviveTeammateCoroutine());
          if (reviveEffectSphere != null) reviveEffectSphere.SetActive(true);
          if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(true);
        }
      }
    }
    else
    {
      // Si el botón se suelta, cancela el revivir
      CancelRevive();
    }
  }

  private void CancelRevive()
  {
    if (reviveCoroutine != null)
    {
      StopCoroutine(reviveCoroutine);
      reviveCoroutine = null;
      // TODO: feedback visual o evento de cancelación
    }
    targetToRevive = null;
    if (reviveEffectSphere != null) reviveEffectSphere.SetActive(false);
    if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(false);
    isReviving = false;
  }

  private PlayerHealth FindDownedTeammate()
  {
    /*
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
    */
    PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

    foreach (var teammateHealth in allPlayers)
    {
      if (teammateHealth != null && teammateHealth != playerHealth && teammateHealth.IsDowned &&
        Vector3.Distance(transform.position, teammateHealth.transform.position) <= reviveDetectRange)
      {
        return teammateHealth;
      }
    }
    return null;
  }

  private IEnumerator ReviveTeammateCoroutine()
  {
    isReviving = true;
    float timer = reviveTime;
    int previousTimerInt = (int)Mathf.Ceil(timer);
    while (timer > 0f)
    {
      int currentTimerInt = (int)Mathf.Ceil(timer);
      if (reviveTimerText != null && currentTimerInt != previousTimerInt)
      {
        reviveTimerText.text = timer.ToString();
        previousTimerInt = currentTimerInt;
      }

      if (targetToRevive == null || !targetToRevive.IsDowned)
      {
        Debug.Log("Revivir cancelado: Objetivo no válido.");
        CancelRevive();
        yield break; // Sale de la corrutina
      }

      Debug.Log("Reviviendo... " + timer + " / " + reviveTime);
      timer -= Time.deltaTime;
      yield return null;
    }

    if (targetToRevive != null)
    {
      targetToRevive.Revive(reviveAmount);
      targetToRevive = null;
    }
    CancelRevive();
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, reviveDetectRange);
  }
}
