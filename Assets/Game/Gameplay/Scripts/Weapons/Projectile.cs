using UnityEngine;

public class Projectile : MonoBehaviour
{
  [Header("Stats")]
  [SerializeField] private float speed = 20f;
  [SerializeField] private float lifeTime = 2f;
  [SerializeField] private float damage = 10f;
  [Header("Colisión")]
  [SerializeField] private LayerMask hitMask = ~0; // Paredes + Enemigos

  private Vector3 direction = Vector3.zero;
  private float timer = 0f;
  private ProjectileObjectPool pool;

  public void SetDirection(Vector3 dir)
  {
    direction = dir.sqrMagnitude > 0f ? dir.normalized : Vector3.zero;
  }

  public void SetPool(ProjectileObjectPool p) => pool = p;

  private void OnEnable()
  {
    timer = 0f;
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
    // Fallback si aparece dentro de un collider
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
    // Para paredes u otros: solo retorno (ya se hace fuera)
  }

  private void ReturnToPool()
  {
    direction = Vector3.zero;
    if (pool != null) pool.ReturnToPool(this);
    else gameObject.SetActive(false);
  }

  private void OnDisable()
  {
    direction = Vector3.zero;
    timer = 0f;
  }

  public float GetDamage() => damage;
}
