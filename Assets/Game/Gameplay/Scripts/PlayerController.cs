using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private float speed = 0f;

    private PlayerInputController inputController = null;
    private CharacterController characterController = null;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
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
}
