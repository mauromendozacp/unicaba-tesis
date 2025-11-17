using UnityEngine;
using System;
using UnityEngine.AI;
using System.Collections;

public enum MiniDragonAttackType { NONE, BITE, TAIL, FIREBALL_GROUND, FIREBALL_AIR }
//public enum CombatStance { NEUTRAL, ENRAGED }

public class MiniDragonController : EnemyBase
{
  [Header("== ESTADÍSTICAS Y DAÑO ==")]
  public float basicAttackDamage = 20f;
  public float tailAttackDamage = 25f;
  public float fireballDamageNeutral = 10f;
  public float fireballDamageEnraged = 15f;

  public EnemyAttack biteAttack;
  public EnemyAttack tailAttack;

  [SerializeField, Tooltip("Porcentaje de vida para entrar en Furia.")]
  private float enragedHealthThreshold = 0.5f;

  [Header("== COMPONENTES Y PUNTOS ==")]
  [SerializeField] private Animator _animator;
  public Animator Animator => _animator;
  [SerializeField] private Collider _meleeCollider;
  public Collider MeleeCollider => _meleeCollider;
  public Transform FireballSpawnPoint;
  public Rigidbody Rb => rb;

  [Header("== LÍMITES Y MOVIMIENTO ==")]
  [SerializeField, Tooltip("Posición a la que viaja el dragón al despertar.")]
  private Transform initialDestination;
  public Vector3 InitialDestination => initialDestination != null ? initialDestination.position : transform.position;
  [SerializeField] private float minMeleeDistance = 3f;
  public float MinMeleeDistance => minMeleeDistance;

  // Velocidades Base
  [Header("== VELOCIDADES DE COMBATE ==")]
  [SerializeField] private float neutralRunSpeed = 6f;
  [SerializeField] private float neutralFlySpeed = 10f;
  [SerializeField] private float neutralRotationSpeed = 10f;
  [SerializeField] private float enragedRunSpeed = 9f;
  [SerializeField] private float enragedFlySpeed = 15f;

  // Altura de vuelo (constante ya que siempre vuela bajo)
  [SerializeField] private float flyHeight = 2.5f;
  public float FlyHeight => flyHeight;

  [HideInInspector] public float airMoveSpeed;
  [HideInInspector] public float groundMoveSpeed;
  [HideInInspector] public float rotationSpeed;

  // Cooldowns
  [Header("== COOLDOWNS ==")]
  [SerializeField] private float meleeCooldown = 1.5f;
  public float MeleeCooldown => meleeCooldown;
  [SerializeField] private float rangedCooldownNeutral = 2.0f;
  [SerializeField] private float rangedCooldownEnraged = 1.0f;

  // Proyectiles
  [Header("== FIREBALLS ==")]
  [SerializeField] private float fireballSpeedNeutral = 15f;
  [SerializeField] private float fireballSpeedEnraged = 20f;
  [SerializeField] private int fireballCountNeutral = 1;
  [SerializeField] private int fireballCountEnraged = 3;
  public int FireballCount { get; private set; }
  public float FireballSpeed { get; private set; }
  public float RangedCooldown { get; private set; }
  public float FireballDamage { get; private set; }

  // -- Lógica de Estado --
  private IState currentState;
  public IState CurrentState => currentState;
  public MiniDragonAttackType CurrentAttack { get; set; } = MiniDragonAttackType.NONE;
  public CombatStance CurrentStance { get; private set; } = CombatStance.NEUTRAL;
  public NavMeshAgent Agent { get; private set; } // Referencia conveniente

  private MiniDragonStateFactory stateFactory;

  // Timers de Cooldown (persistentes)
  [HideInInspector] public float lastMeleeTime;
  [HideInInspector] public float lastRangedTime;

  // --- Lógica de Combate ---
  public override Transform CurrentTarget => currentTargetDamageable != null && currentTargetDamageable.IsAlive ? (currentTargetDamageable as MonoBehaviour).transform : null;

  public event Action OnDragonMiniBossDeath;

  [SerializeField] public AudioEvent miniDragonRoarSound = null;
  [SerializeField] public AudioEvent fireballSound = null;
  [SerializeField] public AudioEvent deathSound = null;

  protected override void Awake()
  {
    base.Awake();
    Agent = GetComponent<NavMeshAgent>();
    stateFactory = new MiniDragonStateFactory(this);
    lastMeleeTime = Time.time;
    lastRangedTime = Time.time;
    //ChangeStance(CombatStance.NEUTRAL); // Inicializa las velocidades
    biteAttack.SetDamage(basicAttackDamage);
    biteAttack.ToggleCollider(false);
    tailAttack.SetDamage(tailAttackDamage);
    tailAttack.ToggleCollider(false);
  }

  void OnEnable()
  {
    ///if (FlameCollider != null) FlameCollider.enabled = false;
    //if (BiteCollider != null) BiteCollider.enabled = false;
    ChangeState(stateFactory.Sleeping());
    ChangeStance(CombatStance.NEUTRAL);
  }

  void Update()
  {
    currentState?.Tick();
  }

  // --- Métodos de Control ---

  public void ChangeState(IState newState)
  {
    currentState?.OnExit();
    currentState = newState;
    currentState.OnEnter();
    Debug.Log($"#{++stateChangeCounter} Estado del Dragón cambiado a: {newState.GetType().Name}. Target actual: {(CurrentTarget != null ? CurrentTarget.name : "Ninguno")}");
  }

  public void ChangeStance(CombatStance stance)
  {
    CurrentStance = stance;

    // Ajustar velocidades y propiedades según la fase
    switch (CurrentStance)
    {
      case CombatStance.NEUTRAL:
        SetSpeed(neutralRunSpeed);
        airMoveSpeed = neutralFlySpeed;
        groundMoveSpeed = neutralRunSpeed;
        rotationSpeed = neutralRotationSpeed;
        Agent.speed = neutralRunSpeed;
        RangedCooldown = rangedCooldownNeutral;
        FireballSpeed = fireballSpeedNeutral;
        FireballCount = fireballCountNeutral;
        FireballDamage = fireballDamageNeutral;
        break;
      case CombatStance.ENRAGED:
        SetSpeed(enragedRunSpeed);
        airMoveSpeed = enragedFlySpeed;
        groundMoveSpeed = enragedRunSpeed;
        Agent.speed = enragedRunSpeed;
        RangedCooldown = rangedCooldownEnraged;
        FireballSpeed = fireballSpeedEnraged;
        FireballCount = fireballCountEnraged;
        FireballDamage = fireballDamageEnraged;
        break;
    }
  }

  [ContextMenu("Ejecutar TriggerWakeUp")]
  // Llamado por un trigger externo para iniciar el combate
  public void TriggerWakeUp()
  {
    if (currentState.GetType() == typeof(MiniDragonSleepingState))
    {
      ChangeState(stateFactory.InitialTravel());
    }
  }

  public override void TakeDamage(float damage)
  {
    if (currentState is MiniDragonSleepingState)
    {
      Debug.Log("Estaba durmiendo ahora se despierta");
      TriggerWakeUp();
      return;
    }

    currentHealth -= damage;
    if (healthBar != null)
    {
      healthBar.UpdateHealthBar();
    }
    StartCoroutine(AppyDamageFeedback());

    if (currentHealth <= 0)
    {
      ChangeState(stateFactory.Die());
      return;
    }

    if (CurrentStance == CombatStance.NEUTRAL && currentHealth <= maxHealth * enragedHealthThreshold)
    {
      Debug.Log("Minidragón entra en modo Furia!");
      ChangeStance(CombatStance.ENRAGED);
    }
  }


  public void MoveToTarget()
  {
    if (CurrentTarget != null)
    {
      MoveTo(CurrentTarget.position);
    }
  }


  public bool IsTargetInMeleeRange()
  {
    return CurrentTarget != null && DistanceToTarget() <= MinMeleeDistance;
  }

  public bool IsTargetInChaseRange()
  {
    return CurrentTarget != null && DistanceToTarget() <= ChaseRadius;
  }

  private IEnumerator AppyDamageFeedback()
  {
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.05f);
    ToggleDamageMaterial(false);

  }

  public override void Kill()
  {
    ChangeState(stateFactory.Die());
  }

  public override Transform FindNearestPlayer()
  {
    currentTargetDamageable = null;
    int numColliders = Physics.OverlapSphereNonAlloc(transform.position, chaseRadius, hitColliders, LayerMask.GetMask("Player"));

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
        currentTargetDamageable = damageable;
      }
    }

    return CurrentTarget;
  }

  public override void Die()
  {
    OnDragonMiniBossDeath?.Invoke();
    rb.isKinematic = true;
    //enabled = false;
    //_hitBoxCollider.enabled = false;
    transform.Find("MinimapIcon")?.gameObject.SetActive(false);
  }

  public void FireSingleBall(Vector3 position, Vector3 direction)
  {
    GameObject fireballGO = FireballPoolManager.Instance.GetFireball();
    Fireball fireball = fireballGO.GetComponent<Fireball>();
    fireball.SetDamage(FireballDamage);
    fireball.Launch(position, direction, FireballSpeed);
  }

  /* Debug */
  [HideInInspector] public int stateChangeCounter = 0;

  protected virtual void OnDrawGizmosSelected()
  {
    // Visualización del Radio de Persecución (ChaseRadius)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, chaseRadius);
  }
}