using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;

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


  public float AttackCooldown => attackCooldown;

  [SerializeField] float knockbackForceMultiplier = 0.5f;

  [Header("Attack Settings")]
  [SerializeField] protected Collider attackCollider;
  [SerializeField] protected float attackDamage = 40f;
  [SerializeField] protected float attackCooldown = 1f;

  [SerializeField] float attackRange = 2f;
  public float AttackRange => attackRange;
  [SerializeField] float chaseRadius = 10f;
  public float ChaseRadius => chaseRadius;

  private Rigidbody rb;
  protected float lastDamage;

  public Transform CurrentTarget { get; set; }

  protected IEnemyState currentState;

  public bool IsAlive => currentHealth > 0;

  public event Action<float, IDamageable> OnDamaged;
  public event Action<IDamageable> OnDeath;

  private IObjectPool<GameObject> parentPool;

  protected NavMeshAgent agent;
  protected Collider selfCollider;

  private Collider[] hitColliders = new Collider[4]; // Array pre-asignado

  protected virtual void Awake()
  {
    currentSpeed = moveSpeed * UnityEngine.Random.Range(0.75f, 1.0f);
    currentHealth = maxHealth;
    rb = GetComponent<Rigidbody>();
    agent = GetComponent<NavMeshAgent>();
    selfCollider = GetComponent<Collider>();

    if (agent != null)
    {
      agent.speed = currentSpeed;
      agent.stoppingDistance = 0.1f;
      agent.updateRotation = true; // El agente maneja la rotación
    }
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

  public void MoveTo(Vector3 targetPosition)
  {
    if (agent != null && agent.enabled && agent.isOnNavMesh)
    {
      agent.isStopped = false;
      agent.SetDestination(targetPosition);
    }
  }

  public void StopMovement()
  {
    if (agent != null && agent.enabled && agent.isOnNavMesh)
    {
      agent.isStopped = true;
    }
  }

  /*
    public void ActiveMovement()
    {
      if (agent != null && agent.enabled && agent.isOnNavMesh)
      {
        agent.isStopped = false;
      }
    }
    */

  /*
    void OnCollisionEnter(Collision collision)
    {
      //ResetKnockbackForce();
      if (currentState.State == EnemyState.Damaged)
      {
        ResetKnockbackForce();
      }
    }
    */

  // Métodos para aplicar y resetear la fuerza de retroceso al recibir el disparo
  public void ApplyKnockbackForce()
  {
    DisableMovementAndCollisions();
    /*if (agent != null)
    {
      agent.enabled = false; // Desactivar agente para permitir la física de knockback
    }*/
    if (rb != null)
    {
      rb.isKinematic = false; // Asegurar que el Rigidbody pueda recibir la fuerza
      rb.AddForce(-transform.forward * lastDamage * knockbackForceMultiplier, ForceMode.Impulse);
    }
  }

  public void ResetKnockbackForce()
  {
    if (rb != null)
    {
      rb.isKinematic = false;
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.isKinematic = true;
    }
    if (agent != null)
    {
      agent.enabled = true; // Reactivar agente
      agent.isStopped = false; // Asegurarse de que pueda volver a moverse
                               // Al reactivar, el estado Chase o Idle se encargará de darle un nuevo destino
    }
    if (selfCollider != null)
    {
      selfCollider.enabled = true;
    }
  }

  public virtual void Die()
  {
    //OnDeath?.Invoke(this);
    //EnemyManager.Instance.OnEnemyKilled();
    // TODO: Obtimizar la mamera en que se informa cuando un enemigo muere
    if (parentPool != null)
    {
      OnDeath?.Invoke(this);
      EnemyManager.Instance.OnEnemyKilled(transform.position);
      parentPool.Release(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public void DisableMovementAndCollisions()
  {
    if (agent != null)
    {
      agent.isStopped = true;
      agent.enabled = false;
    }

    if (selfCollider != null)
    {
      selfCollider.enabled = false;
    }
  }



  public void EnableMovementAndCollisions()
  {
    if (rb != null)
    {
      rb.isKinematic = false;
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
      rb.isKinematic = true;
    }

    if (agent != null)
    {
      agent.enabled = true;
      agent.isStopped = false;
    }

    selfCollider.enabled = true;
  }


  public Transform FindNearestPlayer()
  {
    // Filtrado eficiente
    int numColliders = Physics.OverlapSphereNonAlloc(transform.position, chaseRadius, hitColliders, LayerMask.GetMask("Player"));

    Transform closestTarget = null;
    float minSqrDistance = float.MaxValue;

    for (int i = 0; i < numColliders; i++)
    {
      IDamageable damageable = hitColliders[i].GetComponent<IDamageable>();
      if (damageable == null || !damageable.IsAlive) continue;

      Transform target = hitColliders[i].transform;

      Vector3 directionToTarget = target.position - transform.position;
      float sqrDistance = directionToTarget.sqrMagnitude;

      if (sqrDistance < minSqrDistance)
      {
        minSqrDistance = sqrDistance;
        closestTarget = target;
      }
    }

    return closestTarget;
  }

  public float DistanceToTarget()
  {
    return Vector3.Distance(transform.position, CurrentTarget.position);
  }
}
