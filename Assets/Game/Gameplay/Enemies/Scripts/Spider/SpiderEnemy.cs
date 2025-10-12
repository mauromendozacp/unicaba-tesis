using UnityEngine;

public class SpiderEnemy : EnemyBase
{
  SpiderAnimationController animator;
  public SpiderAnimationController Animator => animator;


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
    ChangeState(new SpiderDamagedState(this));

    /*
    if (currentHealth <= 0)
    {
      ChangeState(new SpiderDeathState(this));
    }
    else
    {
      ChangeState(new SpiderDamagedState(this));
    }
    */
  }


  public override void Kill()
  {
    ChangeState(new SpiderDeathState(this));
  }
}