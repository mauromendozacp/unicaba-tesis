
using UnityEngine;

public enum EnemyState
{
  Idle,
  Chase,
  Attack,
  Damaged,
  Death
}

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
  [SerializeField] protected float maxHealth = 30f;
  protected float currentHealth;

  [SerializeField] float moveSpeed = 4f;
  public float currentSpeed;
  public float CurrentSpeed => currentSpeed;
  [SerializeField] float attackRange = 2f;
  public float AttackRange => attackRange;
  [SerializeField] float chaseRadius = 10f;
  public float ChaseRadius => chaseRadius;
  [SerializeField] float attackCooldown = 1f;
  public float AttackCooldown => attackCooldown;

  [SerializeField] float knockbackForceMultiplier = 0.5f;

  private Rigidbody rb;
  protected float lastDamage;

  public Transform CurrentTarget { get; set; }

  protected IEnemyState currentState;

  public bool IsAlive => currentHealth > 0;

  protected virtual void Awake()
  {
    currentSpeed = moveSpeed * Random.Range(0.75f, 1.0f);
    currentHealth = maxHealth;
    rb = GetComponent<Rigidbody>();
  }

  protected virtual void Update()
  {
    currentState?.Update();
  }

  public void ChangeState(IEnemyState newState)
  {

    currentState?.Exit();
    currentState = newState;
    currentState.Enter();
  }

  public abstract void TakeDamage(float damage);

  void OnCollisionEnter(Collision collision)
  {
    ResetKnockbackForce();
  }

  // Métodos para aplicar y resetear la fuerza de retroceso al recibir el disparo
  public void ApplyKnockbackForce()
  {
    if (rb != null)
    {
      rb.AddForce(-transform.forward * lastDamage * knockbackForceMultiplier, ForceMode.Impulse);
    }
  }

  public void ResetKnockbackForce()
  {
    if (rb != null)
    {
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
    }
  }
}
