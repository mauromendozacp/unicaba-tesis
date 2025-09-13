using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float speed = 0f;
    [SerializeField] private PlayerItemDetection itemDetection = null;

    [Header("Weapon System")]
    [SerializeField] private WeaponHolder weaponHolder;

    private PlayerInputController inputController = null;
    private CharacterController characterController = null;
    private PlayerInventory inventory = null;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        characterController = GetComponent<CharacterController>();
        inventory = GetComponent<PlayerInventory>();

        // weaponHolder puede estar en el mismo GameObject o como hijo
        if (weaponHolder == null)
            weaponHolder = GetComponentInChildren<WeaponHolder>();
    }

    private void Start()
    {
        inventory.Init(inputController);

        inputController.onEquipItem += EquipItem;
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

    private void EquipItem()
    {
        IEquipable itemEquipable = itemDetection.GetFirstItemDetection();
        if (itemEquipable != null)
        {
            inventory.EquipItem(itemEquipable.GetItem());
            itemEquipable.Equip();
        }
    }
}
