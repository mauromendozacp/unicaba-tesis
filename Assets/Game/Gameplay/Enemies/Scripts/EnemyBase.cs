using System;
using System.Collections.Generic;
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

  [Header("Movement Settings")]
  [SerializeField] protected bool isTerritorial = false;
  public bool IsTerritorial => isTerritorial;
  [SerializeField] float moveSpeed = 4f;
  protected Vector3 initialPosition;
  public float currentSpeed;
  public float CurrentSpeed => currentSpeed;

  [Header("Damage Settings")]
  [SerializeField] float knockbackForceMultiplier = 0.5f;
  [SerializeField] protected List<Renderer> damageRenderer;
  [SerializeField] protected Material damagedMaterial;
  protected Material originalMaterial;

  [Header("Attack Settings")]
  [SerializeField] protected Collider attackCollider;
  public float AttackCooldown => attackCooldown;
  [SerializeField] protected float attackDamage = 40f;
  [SerializeField] protected float attackCooldown = 1f;

  [SerializeField] float attackRange = 2f;
  public float AttackRange => attackRange;
  [SerializeField] float chaseRadius = 10f;
  public float ChaseRadius => chaseRadius;
  [SerializeField] float maxChaseDistance = 15f;

  private Rigidbody rb;
  protected float lastDamage;

  public Transform CurrentTarget { get; set; }
  protected IDamageable currentTargetDamageable = null;

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
    rb = GetComponent<Rigidbody>();
    agent = GetComponent<NavMeshAgent>();
    selfCollider = GetComponent<Collider>();
  }

  protected virtual void Start()
  {
    currentSpeed = moveSpeed * UnityEngine.Random.Range(0.75f, 1.0f);
    currentHealth = maxHealth;
    initialPosition = transform.position;
    if (agent != null)
    {
      agent.speed = currentSpeed;
      agent.stoppingDistance = 0.1f;
      //agent.stoppingDistance = attackRange * 0.8f;
      agent.updateRotation = true; // El agente maneja la rotación
    }
    if (damageRenderer != null && damageRenderer.Count > 0 && damagedMaterial != null)
    {
      originalMaterial = damageRenderer[0].material;
    }
  }

  public void ToggleDamageMaterial(bool active)
  {
    if (damageRenderer != null && damagedMaterial != null && originalMaterial != null)
    {
      foreach (var renderer in damageRenderer)
      {
        renderer.material = active ? damagedMaterial : originalMaterial;
      }
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

  public void ToggleSelfCollider(bool active)
  {
    if (selfCollider != null)
    {
      selfCollider.enabled = active;
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
        currentTargetDamageable = damageable;
      }
    }

    return closestTarget;
  }

  public bool isTargetAlive()
  {
    return currentTargetDamageable != null && currentTargetDamageable.IsAlive;
  }

  public float DistanceToTarget()
  {
    return Vector3.Distance(transform.position, CurrentTarget.position);
  }

  public abstract void Kill();

  public void LookAtTarget()
  {
    if (CurrentTarget != null)
    {
      Vector3 direction = (CurrentTarget.position - transform.position).normalized;
      direction.y = 0; // Mantener la rotación en el plano horizontal
      if (direction != Vector3.zero)
      {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
      }
    }
  }

  public Vector3 GetInitialPosition()
  {
    return initialPosition;
  }

  protected virtual void OnDrawGizmosSelected()
  {
    // Visualización del Radio de Persecución (ChaseRadius)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, chaseRadius);

    // Visualización del Límite Territorial (MaxChaseDistance)
    if (isTerritorial)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(initialPosition, maxChaseDistance);
    }
  }

  public bool IsTooFarFromOrigin(float margin = 0f)
  {
    if (!isTerritorial || CurrentTarget == null) return false;

    float effectiveMaxDistance = maxChaseDistance + margin;
    if (effectiveMaxDistance < 0) effectiveMaxDistance = 0;

    float distanceSqr = (CurrentTarget.position - initialPosition).sqrMagnitude;
    float maxDistanceSqr = effectiveMaxDistance * effectiveMaxDistance;

    return distanceSqr > maxDistanceSqr;
  }

  public void SetAttackCollider(bool active)
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = active;
    }
  }
}
