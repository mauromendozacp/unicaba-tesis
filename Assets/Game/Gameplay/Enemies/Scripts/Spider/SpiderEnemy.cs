using UnityEngine;
using System.Collections;

public class SpiderEnemy : EnemySoldier
{
  SpiderAnimationController animator;
  public SpiderAnimationController Animator => animator;
  [SerializeField] Collider armatureCollider;


  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new SpiderIdleState(this));

    if (selfCollider != null) selfCollider.enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    //animator = GetComponent<SpiderAnimationController>();
    base.Awake();
    //ChangeState(new SpiderIdleState(this));
  }

  protected override void Start()
  {
    base.Start();
    animator = GetComponent<SpiderAnimationController>();
    //ChangeState(new SpiderIdleState(this));
    SetAttackCollider(false);
  }

  void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("Player")) return;
    IDamageable player = other.GetComponent<IDamageable>();
    if (player != null && !player.IsAlive)
    {
      CurrentTarget = null;
      ChangeState(new SpiderIdleState(this));
    }

    if (attackCollider != null && attackCollider.enabled && player != null)
    {
      player.TakeDamage(attackDamage);
    }
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
    //yield break;
  }


  public override void Kill()
  {
    ChangeState(new SpiderDeathState(this));
    //ToggleDamageMaterial(false);
  }

  public override void DisableMovementAndCollisions()
  {
    base.DisableMovementAndCollisions();
    if (armatureCollider != null)
    {
      armatureCollider.enabled = false;
    }
  }

  public override void EnableMovementAndCollisions()
  {
    base.EnableMovementAndCollisions();
    if (armatureCollider != null)
    {
      armatureCollider.enabled = true;
    }
  }
}