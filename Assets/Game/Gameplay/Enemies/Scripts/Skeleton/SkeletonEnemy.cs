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
    animator = GetComponent<SkeletonAnimationController>();
    base.Awake();
    ChangeState(new SkeletonIdleState(this));
  }

  void Start()
  {
    SetAttackCollider(false);
  }


  public override void TakeDamage(float damage)
  {
    base.TakeDamage(damage);

    if (currentHealth <= 0)
    {
      ChangeState(new SkeletonDeathState(this));
    }
    else
    {
      ChangeState(new SkeletonDamagedState(this));
    }
  }

  public void SetAttackCollider(bool active)
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = active;
    }
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