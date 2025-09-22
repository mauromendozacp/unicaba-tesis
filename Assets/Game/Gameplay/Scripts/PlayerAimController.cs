using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerAimController : MonoBehaviour
{
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
        SnapToPointer();
    }

    private void Update()
    {
        if (cam == null) return;

        Vector2 screen = input.GetPointerScreenPosition();
        Ray ray = cam.ScreenPointToRay(screen);

        if (Physics.Raycast(ray, out var hit, 1000f, aimLayers, QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, new Vector3(rotateTarget.position.y, 0f, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                aimPoint = ray.origin + ray.direction * enter;
            }
            else
            {
                aimPoint = rotateTarget.position + rotateTarget.forward * 100f;
            }
        }

        Vector3 flat = aimPoint - rotateTarget.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.0001f)
        {
            aimDirection = flat.normalized;
            Quaternion look = Quaternion.LookRotation(aimDirection, Vector3.up);
            rotateTarget.rotation = look;
            if (fireRoot != rotateTarget) fireRoot.rotation = look;
        }
    }

    private void SnapToPointer()
    {
        if (cam == null) return;

        Vector2 screen = input.GetPointerScreenPosition();
        Ray ray = cam.ScreenPointToRay(screen);

        if (Physics.Raycast(ray, out var hit, 1000f, aimLayers, QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, new Vector3(rotateTarget.position.y, 0f, 0f));
            if (plane.Raycast(ray, out float enter))
            {
                aimPoint = ray.origin + ray.direction * enter;
            }
            else
            {
                aimPoint = rotateTarget.position + rotateTarget.forward * 100f;
            }
        }

        Vector3 flat = aimPoint - rotateTarget.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.0001f)
        {
            aimDirection = flat.normalized;
            Quaternion look = Quaternion.LookRotation(aimDirection, Vector3.up);
            rotateTarget.rotation = look;
            if (fireRoot != rotateTarget) fireRoot.rotation = look;
        }
    }
}
