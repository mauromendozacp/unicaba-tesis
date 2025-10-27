using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DragonBabyRanged : EnemySoldier
{
  [Header("COMPONENTES")]
  [SerializeField] DragonBabyRangedAnimator _animator;
  public DragonBabyRangedAnimator Animator => _animator;

  [Header("== CONFIGURACIÓN DE RANGO ==")]
  [SerializeField] Transform firePoint;
  [SerializeField] float fireBallSpeedNeutral = 12f;
  [SerializeField] float optimalAttackDistance = 10f;
  [SerializeField] float attackRangeMargin = 2f;


  // Propiedades públicas para la IA
  public float OptimalAttackDistance => optimalAttackDistance;
  public float AttackRangeMargin => attackRangeMargin;
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
  }

  protected override void Start()
  {
    base.Start();
    //ChangeState(new DragonBabyRangedIdleState(this));
    CurrentFireBallSpeed = fireBallSpeedNeutral;
  }


  // --- LÓGICA DE ATAQUE ---
  public void ShootFireBall()
  {
    if (FireballPoolManager.Instance == null || firePoint == null) return;

    lastAttackTime = Time.time;

    GameObject fireBall = FireballPoolManager.Instance.GetFireball();
    fireBall.transform.position = firePoint.position;

    Rigidbody rb = fireBall.GetComponent<Rigidbody>();
    if (CurrentTarget != null)
    {
      Vector3 direction = (CurrentTarget.position - firePoint.position).normalized;
      rb.linearVelocity = direction * CurrentFireBallSpeed;
      fireBall.transform.rotation = Quaternion.LookRotation(direction);

      Fireball fireballScript = fireBall.GetComponent<Fireball>();
      // Asume que Fireball tiene un campo 'damage'
      //fireballScript.damage = IsEnraged ? attackDamage * 1.5f : attackDamage;
      rb.useGravity = false;
    }
    else
    {
      FireballPoolManager.Instance.ReleaseFireball(fireBall);
    }
  }

  // --- LÓGICA DE DAÑO Y MATERIALES ---
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
    yield break;
  }

  public override void Kill()
  {
    ChangeState(new DragonBabyRangedDeathState(this));
    ToggleDamageMaterial(false);
  }
}