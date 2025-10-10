using UnityEngine;

public class Projectile : MonoBehaviour
{
  [Header("Stats")]
  [SerializeField] private float speed = 20f;
  [SerializeField] private float lifeTime = 2f;
  [SerializeField] private float damage = 10f;
  [Header("Colisión")]
  [SerializeField] private LayerMask hitMask = ~0; // Paredes + Enemigos
  [Header("Trail")]
  [SerializeField] private TrailRenderer trail;
  [SerializeField] private BulletTrailScriptableObject trailConfig;

  private Vector3 direction = Vector3.zero;
  private float timer = 0f;
  private ProjectileObjectPool pool;

  public void SetPool(ProjectileObjectPool pool)
  {
    this.pool = pool;
  }

  private void OnEnable()
  {
    timer = 0f;
    if (trail != null)
    {
      trail.emitting = false;
      trail.Clear();
    }
  }

  public void SetDirection(Vector3 dir)
  {
    direction = dir.sqrMagnitude > 0f ? dir.normalized : Vector3.zero;
    
    if (trail != null)
    {
      if (trailConfig != null) trailConfig.SetupTrail(trail);
      Invoke(nameof(EnableTrail), 0.02f);
    }
  }

  private void EnableTrail()
  {
    if (trail != null) trail.emitting = true;
  }

  private void Update()
  {
    if (direction != Vector3.zero)
    {
      Vector3 step = direction * speed * Time.deltaTime;

      // Raycast anti túnel
      if (Physics.Raycast(transform.position, direction, out RaycastHit hit, step.magnitude, hitMask, QueryTriggerInteraction.Ignore))
      {
        HandleHit(hit.collider);
        ReturnToPool();
        return;
      }

      transform.position += step;
    }

    timer += Time.deltaTime;
    if (timer >= lifeTime)
    {
      ReturnToPool();
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    HandleHit(other);
    ReturnToPool();
  }

  private void HandleHit(Collider col)
  {
    if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
    {
      var enemy = col.GetComponent<EnemyBase>();
      if (enemy != null && enemy.IsAlive)
        enemy.TakeDamage(damage);
    }
  }

  private void ReturnToPool()
  {
    direction = Vector3.zero;
    if (trail != null)
    {
      trail.emitting = false;
      trail.Clear();
    }
    if (pool != null) pool.ReturnToPool(this);
    else gameObject.SetActive(false);
  }

  private void OnDisable()
  {
    direction = Vector3.zero;
    timer = 0f;
    if (trail != null)
    {
      trail.emitting = false;
      trail.Clear();
    }
  }

  public float GetDamage() => damage;
}
