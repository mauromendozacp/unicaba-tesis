using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
  [Header("General Settings")]
  [SerializeField] private float speed = 0f;

  [Header("Weapon System")]
  [SerializeField] private WeaponHolder weaponHolder;

  private PlayerInputController inputController = null;
  private CharacterController characterController = null;

  private Vector3 velocity = Vector3.zero;

  private void Awake()
  {
    inputController = GetComponent<PlayerInputController>();
    characterController = GetComponent<CharacterController>();
    // weaponHolder puede estar en el mismo GameObject o como hijo
    if (weaponHolder == null)
      weaponHolder = GetComponentInChildren<WeaponHolder>();
  }

  private void Update()
  {
    Move();
    HandleFireInput();
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
    }
  }
}
