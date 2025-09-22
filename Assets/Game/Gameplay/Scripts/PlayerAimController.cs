using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private LayerMask aimLayers = ~0;
    [SerializeField] private float stickDeadzone = 0.2f;

    private PlayerInputController input;
    private Camera cam;
    private Vector3 aimPoint;
    private Vector3 aimDirection;

    public Vector3 AimPoint => aimPoint;
    public Vector3 AimDirection => aimDirection;

    private void Awake()
    {
        input = GetComponent<PlayerInputController>();
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null) return;

        Vector2 stick = input.GetRightStickAim();
        if (stick.sqrMagnitude >= stickDeadzone * stickDeadzone)
        {
            Vector3 dir = StickToWorldDirection(stick, cam);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                aimDirection = dir.normalized;
                transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                aimPoint = transform.position + aimDirection * 100f;
                return;
            }
        }

        Vector2 screen = input.GetPointerScreenPosition();
        Ray ray = cam.ScreenPointToRay(screen);

        if (Physics.Raycast(ray, out var hit, 1000f, aimLayers, QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                aimPoint = ray.origin + ray.direction * enter;
            }
            else
            {
                aimPoint = transform.position + transform.forward * 100f;
            }
        }

        Vector3 flat = aimPoint - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.0001f)
        {
            aimDirection = flat.normalized;
            transform.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
        }
    }

    private static Vector3 StickToWorldDirection(Vector2 stick, Camera c)
    {
        Vector3 f = c.transform.forward;
        Vector3 r = c.transform.right;
        f.y = 0f;
        r.y = 0f;
        f.Normalize();
        r.Normalize();
        return r * stick.x + f * stick.y;
    }
}
