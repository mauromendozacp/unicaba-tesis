using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerAimController : MonoBehaviour
{
    [Header("Aim Settings")]
    [SerializeField] private LayerMask aimLayers = ~0;
    [SerializeField] private Transform graphicsRoot;
    [SerializeField] private Transform fireRoot;

    private PlayerInputController input;
    private Camera cam;
    private Transform rotateTarget;
    private Vector3 aimPoint;
    private Vector3 aimDirection;

    public Vector3 AimPoint => aimPoint;
    public Vector3 AimDirection => aimDirection;

    private void Awake()
    {
        input = GetComponent<PlayerInputController>();
        cam = Camera.main;
        rotateTarget = graphicsRoot != null ? graphicsRoot : transform;
        if (fireRoot == null) fireRoot = transform;
    }

    private void Start()
    {
        UpdateAimPosition(); // alineamos al inicio
    }

    private void Update()
    {
        UpdateAimPosition();
    }

    private void UpdateAimPosition()
    {
        if (cam == null) return;

        Vector2 lookInput = input.GetLookInput();
        Vector2 pointerScreen = input.GetPointerScreenPosition();

        bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.001f;
        bool usingStick = lookInput.sqrMagnitude > 0.1f;

        // 🖱 MODO MOUSE: raycast desde puntero
        if (usingMouse || !usingStick)
        {
            Ray ray = cam.ScreenPointToRay(pointerScreen);
            if (Physics.Raycast(ray, out var hit, 1000f, aimLayers, QueryTriggerInteraction.Ignore))
                aimPoint = hit.point;
            else
            {
                Plane plane = new Plane(Vector3.up, new Vector3(rotateTarget.position.y, 0f, 0f));
                if (plane.Raycast(ray, out float enter))
                    aimPoint = ray.origin + ray.direction * enter;
                else
                    aimPoint = rotateTarget.position + rotateTarget.forward * 10f;
            }
        }
        // 🎮 MODO GAMEPAD: usar vector Look
        else if (usingStick)
        {
            aimDirection = new Vector3(lookInput.x, 0, lookInput.y).normalized;
            aimPoint = rotateTarget.position + aimDirection * 5f;
        }

        // Rotar hacia el punto calculado
        Vector3 flat = aimPoint - rotateTarget.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.0001f)
        {
            aimDirection = flat.normalized;
            Quaternion look = Quaternion.LookRotation(aimDirection, Vector3.up);
            rotateTarget.rotation = look;
            if (fireRoot != rotateTarget)
                fireRoot.rotation = look;
        }
    }
}
