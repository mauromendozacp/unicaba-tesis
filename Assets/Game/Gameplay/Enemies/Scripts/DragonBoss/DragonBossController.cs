using UnityEngine;
using System.Collections;

public class DragonBossController : EnemyBase
{
  [Header("Componentes")]
  [SerializeField] Animator _animator;
  [SerializeField] Collider _hitBoxCollider; // El collider que recibe daño
  [SerializeField] Collider _flameCollider; // Collider del ataque de fuego
  [SerializeField] Collider _biteCollider; // Collider del ataque de mordida
  [SerializeField] public GameObject flame;

  public Animator Animator => _animator;
  public Collider HitBoxCollider => _hitBoxCollider;
  public Collider FlameCollider => _flameCollider;
  public Collider BiteCollider => _biteCollider;

  [Header("Estadísticas")]
  public float groundMoveSpeed = 4f;
  public float airMoveSpeed = 8f;
  public float rotationSpeed = 10f;

  public override Transform CurrentTarget => currentTargetDamageable != null && currentTargetDamageable.IsAlive ? (currentTargetDamageable as MonoBehaviour).transform : null;

  IState currentState;
  DragonStateFactory stateFactory;

  public Rigidbody Rb => rb;

  protected override void Awake()
  {
    base.Awake();
    stateFactory = new DragonStateFactory(this);
    rb.freezeRotation = true; // Previene que la física lo rote inesperadamente
  }

  void OnEnable()
  {
    if (FlameCollider != null) FlameCollider.enabled = false;
    if (BiteCollider != null) BiteCollider.enabled = false;
    ChangeState(stateFactory.GroundIdle());
  }


  protected override void Start()
  {
    base.Start();
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
    Debug.Log($"Estado del Dragón cambiado a: {newState.GetType().Name}");
  }


  public override void TakeDamage(float damage)
  {
    currentHealth -= damage;
    //Debug.Log($"Dragón recibió {damage}. Vida: {currentHealth}");
    StartCoroutine(AppyDamageFeedback());

    if (currentHealth <= 0)
    {
      ChangeState(stateFactory.Die());
    }
    else if (currentHealth <= maxHealth * 0.3f && !IsAirState(currentState))
    {
      // Transición a FASE 3 (Furia) forzada al 30% de vida si está en tierra
      ChangeState(stateFactory.TransitionTakeoff());
    }
  }

  private bool IsAirState(IState state)
  {
    return state is DragonTransitionTakeoffState ||
    state is DragonTransitionLandingState ||
    state is DragonAirHoverAttackState ||
    state is DragonAirFlyRepositionState;
  }

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
    Debug.Log($"DragonBossController: OnTriggerEnter con {other.name}");
    if (other.CompareTag("Player"))
    {
      IDamageable player = other.GetComponent<IDamageable>();
      if (player != null && player.IsAlive)
      {
        player.TakeDamage(25.0f);
      }
    }
  }

  private IEnumerator AppyDamageFeedback()
  {
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.05f);
    ToggleDamageMaterial(false);
  }
}