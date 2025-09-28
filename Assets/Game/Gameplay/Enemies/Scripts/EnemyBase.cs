using System;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyState
{
  Idle,
  Chase,
  Attack,
  Retreat,
  Damaged,
  Death
}

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
  [SerializeField] protected float maxHealth = 30f;
  protected float currentHealth;
  public float Health => currentHealth;

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

  public event Action<float, IDamageable> OnDamaged;
  public event Action<IDamageable> OnDeath;

  private IObjectPool<GameObject> parentPool;

  protected virtual void Awake()
  {
    currentSpeed = moveSpeed * UnityEngine.Random.Range(0.75f, 1.0f);
    currentHealth = maxHealth;
    rb = GetComponent<Rigidbody>();
  }

  public void SetPool(IObjectPool<GameObject> pool)
  {
    parentPool = pool;
  }

  protected virtual void Update()
  {
    currentState?.Update();
  }

  public void ChangeState(IEnemyState newState)
  {

    currentState?.Exit();
    StopAllCoroutines();
    currentState = newState;
    currentState.Enter();
  }

  public virtual void TakeDamage(float damage)
  {
    OnDamaged?.Invoke(damage, this);
    lastDamage = damage;
    currentHealth -= damage;
    if (currentHealth <= 0)
    {
      currentHealth = 0;
    }
  }

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

  public virtual void Die()
  {
    //OnDeath?.Invoke(this);
    //EnemyManager.Instance.OnEnemyKilled();
    if (parentPool != null)
    {
      OnDeath?.Invoke(this);
      EnemyManager.Instance.OnEnemyKilled();
      parentPool.Release(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
