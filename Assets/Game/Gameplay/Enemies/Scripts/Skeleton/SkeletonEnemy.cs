using UnityEngine;

public class SkeletonEnemy : EnemyBase
{
  SkeletonAnimationController animator;
  public SkeletonAnimationController Animator => animator;

  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new SkeletonIdleState(this));

    if (selfCollider != null) selfCollider.enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    //animator = GetComponent<SkeletonAnimationController>();
    base.Awake();
    //ChangeState(new SkeletonIdleState(this));
  }

  protected override void Start()
  {
    base.Start();
    animator = GetComponent<SkeletonAnimationController>();
    //ChangeState(new SkeletonIdleState(this));
    SetAttackCollider(false);
  }


  public override void TakeDamage(float damage)
  {
    if (!IsAlive) return;
    base.TakeDamage(damage);
    ChangeState(new SkeletonDamagedState(this));
    /*
    if (currentHealth <= 0)
    {
      ChangeState(new SkeletonDeathState(this));
    }
    else
    {
      ChangeState(new SkeletonDamagedState(this));
    }
    */
  }


  public override void Kill()
  {
    ChangeState(new SkeletonDeathState(this));
  }

  public void EnableAttackCollider()
  {
    SetAttackCollider(true);
  }

  public void DisableAttackCollider()
  {
    SetAttackCollider(false);
  }
}