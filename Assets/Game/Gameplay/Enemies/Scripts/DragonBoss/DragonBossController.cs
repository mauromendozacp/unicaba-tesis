using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.VFX;

public enum DragonAttackType
{
  GROUND_NONE = 0,
  GROUND_BITE,
  GROUND_FLAME,
  GROUND_FIREBALL,
  AIR_NONE = 10,
  AIR_FIREBALL
}

public enum CombatStance
{
  NEUTRAL = 0,

  // Estados agresivos
  Aggressive,     // Atacando activamente, alta frecuencia de ataques
  ENRAGED,        // Furia total, daño y velocidad maximizados
  FocusAttack,    // Enfocado en un ataque específico o en un objetivo

  // Estados defensivos o de utilidad
  Defensive,      // Recibiendo poco daño, cubriéndose
  Vulnerable,     // Acaba de fallar un ataque, expuesto, recibiendo más daño
  Regeneration,   // Recuperando vida o armadura
  Summoning,      // Invocando esbirros
  Charging        // Cargando un ataque grande o habilidad especial
}
public class DragonBossController : EnemyBase
{

  [Header("Configuración de Arena y Vuelo")]
  [Tooltip("Límites del área de vuelo (X=Mín, Y=Máx) para generar posiciones aleatorias.")]
  [SerializeField] private Vector2 arenaBoundsX = new Vector2(-20f, 20f);
  [Tooltip("Límites del área de vuelo (X=Mín, Y=Máx) para generar posiciones aleatorias.")]
  [SerializeField] private Vector2 arenaBoundsZ = new Vector2(-20f, 20f);
  [Tooltip("Distancia mínima al punto actual para que se considere un nuevo punto de reposicionamiento válido.")]
  [SerializeField] private float minRepositionDistance = 15f;
  [Tooltip("Altura fija a la que el dragón volará y se reposicionará.")]
  [SerializeField] private float flyHeight = 10f;

  public Vector2 ArenaBoundsX => arenaBoundsX;
  public Vector2 ArenaBoundsZ => arenaBoundsZ;
  public float MinRepositionDistance => minRepositionDistance;
  public float FlyHeight => flyHeight;


  [Header("Estadísticas de Ataque / Daño")]
  public float biteDamage = 35f;
  public float flameDamage = 20f;
  public float fireballDamage = 15f;

  [SerializeField][Tooltip("Umbral de vida para entrar en modo furia (30%).")] float enragedHealthThreshold = 0.3f;
  [SerializeField][Tooltip("Umbral de daño acumuladodo para intentar escapar volando (5%) en modo furia.")] float damageToEscapeThreshold = 0.05f;
  [Header("Ataque Aéreo")]
  [SerializeField]
  //[Tooltip("Ajuste de altura del punto al que se apunta para el ataque aéreo.")] public float AirBombTargetElevation = 1.5f;
  float damageToEscape = 0f;
  public bool IsVulnerable;


  [Header("Componentes")]
  [SerializeField] Animator _animator;
  [SerializeField] Collider _hitBoxCollider; // El collider que recibe daño
  [SerializeField] Collider _flameCollider; // Collider del ataque de fuego
  [SerializeField] Collider _biteCollider; // Collider del ataque de mordida
  [SerializeField] public GameObject flame;
  [SerializeField] public VisualEffect flameEffect;

  public Animator Animator => _animator;
  public Collider HitBoxCollider => _hitBoxCollider;
  public Collider FlameCollider => _flameCollider;
  public Collider BiteCollider => _biteCollider;


  [Header("Estadísticas de velocidad")]
  public float neutralGroundMoveSpeed = 4f;
  public float neutralAirMoveSpeed = 8f;
  public float neutralRotationSpeed = 10f;

  public float enragedGroundMoveSpeed = 8f;
  public float enragedAirMoveSpeed = 12f;
  //public float enragedRotationSpeed = 15f;

  [HideInInspector] public float groundMoveSpeed;
  [HideInInspector] public float airMoveSpeed;
  [HideInInspector] public float rotationSpeed;

  [SerializeField] float neutralAttackRotationSpeed = 1f;
  [SerializeField] float enragedAttackRotationSpeed = 1.6f;
  float _attackRotationSpeed;
  public float AttackRotationSpeed => _attackRotationSpeed;

  [SerializeField] float neutralFireballSpeed = 8.0f;
  [SerializeField] float enragedFireballSpeed = 12.0f;

  public float FireballSpeed { get; private set; }

  public Transform FireballSpawnPoint;

  public override Transform CurrentTarget => currentTargetDamageable != null && currentTargetDamageable.IsAlive ? (currentTargetDamageable as MonoBehaviour).transform : null;

  IState currentState;
  CombatStance currentStance = CombatStance.NEUTRAL;
  [HideInInspector] public DragonAttackType CurrentAttack { get; set; } = DragonAttackType.GROUND_NONE;

  DragonStateFactory stateFactory;

  public Rigidbody Rb => rb;

  public event Action OnDragonBossDeath;

  public void ChangeCombatStance(CombatStance stance)
  {
    currentStance = stance;
    switch (currentStance)
    {
      case CombatStance.NEUTRAL:
        groundMoveSpeed = neutralGroundMoveSpeed;
        airMoveSpeed = neutralAirMoveSpeed;
        rotationSpeed = neutralRotationSpeed;
        _attackRotationSpeed = neutralAttackRotationSpeed;
        FireballSpeed = neutralFireballSpeed;
        break;
      case CombatStance.ENRAGED:
        groundMoveSpeed = enragedGroundMoveSpeed;
        airMoveSpeed = enragedAirMoveSpeed;
        //rotationSpeed = enragedRotationSpeed;
        _attackRotationSpeed = enragedAttackRotationSpeed;
        FireballSpeed = enragedFireballSpeed;
        break;
    }
    SetSpeed(groundMoveSpeed);
  }


  protected override void Awake()
  {
    base.Awake();
    stateFactory = new DragonStateFactory(this);
    rb.freezeRotation = true;
  }

  void OnEnable()
  {
    //if (FlameCollider != null) FlameCollider.enabled = false;
    if(FlameCollider != null) FlameCollider.gameObject.SetActive(false);
    if (BiteCollider != null) BiteCollider.enabled = false;
    ChangeState(stateFactory.GroundIdle());
    ChangeCombatStance(CombatStance.NEUTRAL);
  }

  void Update()
  {
    currentState?.Tick();
  }

  public void ChangeState(IState newState)
  {
    currentState?.OnExit();
    currentState = newState;
    currentState.OnEnter();
    //Debug.Log($"#{++stateChangeCounter} Estado del Dragón cambiado a: {newState.GetType().Name}");
  }


  public override void TakeDamage(float damage)
  {
    if (!IsVulnerable) return;
    currentHealth -= damage;
    StartCoroutine(AppyDamageFeedback());

    if (currentHealth <= 0)
    {
      ChangeState(stateFactory.Die());
      return;
    }

    switch (currentStance)
    {
      case CombatStance.ENRAGED:
        damageToEscape -= damage;
        if (!IsAirState(currentState) && damageToEscape <= 0)
        {
          damageToEscape = maxHealth * damageToEscapeThreshold;
          ChangeState(stateFactory.TransitionTakeoff());
          return;
        }
        break;
      case CombatStance.NEUTRAL:
        if (currentHealth <= maxHealth * enragedHealthThreshold)
        {
          Debug.Log("El Dragón entra en modo Furia!");
          ChangeCombatStance(CombatStance.ENRAGED);
        }
        break;
    }
  }

  private bool IsAirState(IState state)
  {
    return state is DragonTransitionTakeoffState ||
    state is DragonTransitionLandingState ||
    state is DragonAirHoverAttackState ||
    state is DragonAirFlyRepositionState;
  }

  /*
    // Método que se llama desde los Eventos de Animación
    public void OnAnimationEvent(string eventName)
    {
      // Permite a los estados manejar eventos específicos de la animación.
      // Ejemplo: Si el estado actual es un ataque, llama a su propio método para generar daño.
      if (currentState is DragonGroundAttackState attackState)
      {
        attackState.HandleAttackEvent(eventName);
      }
      else if (currentState is DragonTransitionTakeoffState takeoffState)
      {
        takeoffState.HandleAnimationEnd();
      }
    }
  */

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

  public override void Kill()
  {
    ChangeState(stateFactory.Die());
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      IDamageable player = other.GetComponent<IDamageable>();
      if (player != null && player.IsAlive)
      {
        switch (CurrentAttack)
        {
          case DragonAttackType.GROUND_BITE:
            player.TakeDamage(biteDamage);
            break;
          case DragonAttackType.GROUND_FLAME:
            player.TakeDamage(flameDamage);
            break;
        }
      }
    }
  }

  private IEnumerator AppyDamageFeedback()
  {
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.05f);
    ToggleDamageMaterial(false);

  }

  public override void Die()
  {
    OnDragonBossDeath?.Invoke();
    rb.isKinematic = true;
    //enabled = false;
    _hitBoxCollider.enabled = false;
    transform.Find("MinimapIcon")?.gameObject.SetActive(false);
  }

  public void FireSingleBall(Vector3 position, Vector3 direction)
  {
    GameObject fireballGO = FireballPoolManager.Instance.GetFireball();
    Fireball fireball = fireballGO.GetComponent<Fireball>();
    fireball.SetDamage(fireballDamage);
    fireball.Launch(position, direction, FireballSpeed);
  }



  /* Debug */
  [HideInInspector] public Vector3 CurrentRepositionTarget { get; set; } = Vector3.zero;
  [HideInInspector] public int stateChangeCounter = 0;

  private void OnDrawGizmos()
  {
    if (CurrentRepositionTarget != Vector3.zero)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawLine(transform.position, CurrentRepositionTarget);
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(CurrentRepositionTarget, 1f);
    }
  }
}