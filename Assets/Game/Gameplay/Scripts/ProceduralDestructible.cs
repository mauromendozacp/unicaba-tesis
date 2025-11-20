using UnityEngine;
using System.Collections.Generic;

public class ProceduralDestructible : MonoBehaviour
{
    [Header("Fragment Prefabs Settings")]
    [Tooltip("Lista de prefabs de madera que se instanciarán aleatoriamente")]
    public List<GameObject> woodFragmentPrefabs;

    [Header("Destruction Settings")]
    [Range(0, 100)]
    public int intactWoodPercentage = 20;

    [Tooltip("Cantidad total de piezas generadas al romperse")]
    public int fragmentCount = 100;

    [Tooltip("Cuánto se extiende la explosión respecto al tamaño de la caja")]
    public float explosionMargin = 0.2f;

    [Tooltip("Fuerza base de la explosión (intensidad del impulso)")]
    public float baseExplosionForce = 20f;

    [Tooltip("Altura relativa del impulso (empuje hacia arriba)")]
    public float upwardModifier = 1.2f;

    [Tooltip("Multiplicador global de la velocidad de la explosión (1 = normal)")]
    [Range(0.1f, 2f)] public float forceMultiplier = 0.25f;

    [Tooltip("Multiplicador del radio de dispersión (1 = normal)")]
    [Range(0.1f, 2f)] public float radiusMultiplier = 0.25f;

    [Tooltip("Escala mínima de fragmentos al romperse (relativa al prefab)")]
    public float minFragmentScale = 0.5f;

    [Tooltip("Escala máxima de fragmentos al romperse (relativa al prefab)")]
    public float maxFragmentScale = 1.8f;

    [Header("Ground Detection")]
    public string groundLayerName = "Floor";
    public float groundOffset = 0.02f;
    public float raycastDistance = 10f;

    [Header("Physics Settings")]
    public float airDrag = 0.5f;
    public float gravityScale = 1f;

    [Tooltip("Tiempo de vida de los fragmentos generados antes de desaparecer.")]
    public float fragmentLifetime = 2f; 

    private bool isBroken = false;
    private int groundLayerMask;

    private void Awake()
    {
        groundLayerMask = LayerMask.GetMask(groundLayerName);
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        if (woodFragmentPrefabs == null || woodFragmentPrefabs.Count == 0)
        {
            Debug.LogWarning($"[ProceduralDestructible] {name}: no hay prefabs asignados.");
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (var rend in renderers)
            bounds.Encapsulate(rend.bounds);

        Vector3 origin = bounds.center;
        Vector3 size = bounds.size;

        // Detectar el suelo
        float groundY = origin.y - size.y / 2f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastDistance, groundLayerMask))
            groundY = hit.point.y + groundOffset;

        // Centro de la explosión lateral
        Vector3 explosionCenter = origin + new Vector3(
            Random.Range(-size.x * 0.3f, size.x * 0.3f),
            size.y * 0.25f,
            Random.Range(-size.z * 0.3f, size.z * 0.3f)
        );

        int intactCount = Mathf.RoundToInt(fragmentCount * (intactWoodPercentage / 100f));

        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject prefab = woodFragmentPrefabs[Random.Range(0, woodFragmentPrefabs.Count)];
            if (prefab == null) continue;

            // Posición inicial dentro del volumen de la caja
            Vector3 localPos = new Vector3(
                Random.Range(-size.x * (0.5f - explosionMargin), size.x * (0.5f - explosionMargin)),
                Random.Range(-size.y * (0.5f - explosionMargin), size.y * (0.5f - explosionMargin)),
                Random.Range(-size.z * (0.5f - explosionMargin), size.z * (0.5f - explosionMargin))
            );

            Vector3 spawnPos = origin + localPos;
            spawnPos.y = Mathf.Max(spawnPos.y, groundY + 0.05f);

            Quaternion rot = Random.rotation;
            GameObject frag = Instantiate(prefab, spawnPos, rot);
            frag.SetActive(true);

            // AUTODESTRUCCIÓN: Destruye el fragmento después del tiempo de vida configurado.
            Destroy(frag, fragmentLifetime); 

            // Tamaño inicial aleatorio
            float randomScaleFactor = Random.Range(minFragmentScale, maxFragmentScale);
            frag.transform.localScale = prefab.transform.localScale * randomScaleFactor;

            // Al caer, vuelve a su tamaño original
            frag.AddComponent<ReturnToOriginalScale>().targetScale = prefab.transform.localScale;

            // Físicas
            Rigidbody rb = frag.GetComponent<Rigidbody>();
            if (rb == null)
                rb = frag.AddComponent<Rigidbody>();

            rb.mass = 0.15f * randomScaleFactor; // fragmentos grandes más pesados
            rb.linearDamping = airDrag;
            rb.angularDamping = 0.6f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.useGravity = true;

            Collider col = frag.GetComponent<Collider>();
            if (col == null)
            {
                MeshCollider meshCol = frag.AddComponent<MeshCollider>();
                meshCol.convex = true;
            }

            // Fuerza errática — direcciones aleatorias y desbalanceadas
            Vector3 randomOffsetDir = (Random.insideUnitSphere + new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(0f, 0.6f),
                Random.Range(-0.3f, 0.3f)
            )).normalized;

            float adjustedForce = baseExplosionForce * forceMultiplier * Random.Range(0.6f, 1.4f);
            float adjustedRadius = Mathf.Max(size.x, size.z) * (1f + explosionMargin) * radiusMultiplier;

            rb.AddExplosionForce(adjustedForce, explosionCenter + randomOffsetDir * 0.2f, adjustedRadius, upwardModifier, ForceMode.Impulse);

            frag.AddComponent<CustomGravity>().gravityScale = gravityScale;
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        foreach (var rend in renderers)
            bounds.Encapsulate(rend.bounds);

        Vector3 origin = bounds.center;
        Vector3 size = bounds.size;
        Vector3 explosionCenter = origin + new Vector3(0, size.y * 0.3f, 0);

        Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f);
        Gizmos.DrawWireCube(origin, size);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(explosionCenter, 0.1f);
    }
#endif
}

/// <summary>
/// Hace que el fragmento vuelva suavemente a su tamaño original tras caer.
/// </summary>
public class ReturnToOriginalScale : MonoBehaviour
{
    public Vector3 targetScale;
    private bool grounded = false;
    private float t = 0f;
    private float speed = 0.5f;

    private void OnCollisionEnter(Collision col)
    {
        if (!grounded && col.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            grounded = true;
        }
    }

    private void Update()
    {
        if (grounded)
        {
            t += Time.deltaTime * speed;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
            if (t >= 1f) Destroy(this);
        }
    }
}