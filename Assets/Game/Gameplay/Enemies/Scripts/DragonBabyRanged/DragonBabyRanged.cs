using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DragonBabyRanged : EnemySoldier
{
  [Header("COMPONENTES Y REFERENCIAS")]
  [SerializeField] DragonBabyRangedAnimator _animator;
  public DragonBabyRangedAnimator Animator => _animator;
  [SerializeField] Transform firePoint;
  [SerializeField] public AudioEvent fireballSound = null; // TODO: Asignar sonido en el prefab de la bola de fuego

  [Header("CONFIGURACIÓN DE RANGO")]
  [SerializeField] float fireBallSpeedNeutral = 12f;
  [SerializeField] float optimalAttackDistance = 10f;
  //[SerializeField] float attackRangeMargin = 2f;
  [SerializeField] float fireballDamage = 15f;


  // Propiedades públicas para la IA
  //public float OptimalAttackDistance => optimalAttackDistance;
  //public float AttackRangeMargin => attackRangeMargin;
  public override float AttackRange => optimalAttackDistance;

  // Velocidades
  public float CurrentFireBallSpeed { get; private set; }

  // Cooldowns
  //public override float AttackCooldown { get; private set; }
  private float lastAttackTime;
  public float LastAttackTime => lastAttackTime;


  void OnEnable()
  {
    ToggleDamageMaterial(false);
    currentHealth = maxHealth;
    ChangeState(new DragonBabyRangedIdleState(this));

    if (selfCollider != null) selfCollider.enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    base.Awake();
    if (_animator == null)
    {
      _animator = GetComponent<DragonBabyRangedAnimator>();
    }
    if (firePoint == null)
    {
      Debug.LogWarning("Fire Point no asignado en " + gameObject.name);
      firePoint = transform;
    }
  }

  protected override void Start()
  {
    base.Start();
    //ChangeState(new DragonBabyRangedIdleState(this));
    CurrentFireBallSpeed = fireBallSpeedNeutral;
  }


  public void ShootFireBall()
  {
    if (FireballPoolManager.Instance == null || CurrentTarget == null) return;
    lastAttackTime = Time.time;
    Vector3 direction = (CurrentTarget.position - firePoint.position).normalized;
    FireSingleBall(firePoint.position, direction);
  }

  public void FireSingleBall(Vector3 position, Vector3 direction)
  {
    GameObject fireballGO = FireballPoolManager.Instance.GetFireball();
    Fireball fireball = fireballGO.GetComponent<Fireball>();
    fireball.SetDamage(fireballDamage);
    fireball.Launch(position, direction, CurrentFireBallSpeed);
  }

  public override void TakeDamage(float damage)
  {
    if (!IsAlive) return;
    base.TakeDamage(damage);
    StartCoroutine(DamageMaterial());
    if (!IsAlive)
    {
      Kill();
    }
  }

  private IEnumerator DamageMaterial()
  {
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.1f);
    ToggleDamageMaterial(false);
    /*
    if (!IsAlive)
    {
      yield break;
    }
    */
  }

  public override void Kill()
  {
    StopAllCoroutines();
    DisableMovementAndCollisions();
    ToggleSelfCollider(false);

    ChangeState(new DragonBabyRangedDeathState(this));
  }
}