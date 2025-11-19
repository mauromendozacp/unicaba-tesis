using UnityEngine;

public class DragonBabyRangedChaseState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedChaseState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Chase;
  }

  public void Enter()
  {
    enemy.Animator.ToggleRun(true);
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
      return;
    }

    float distanceToTarget = enemy.DistanceToTarget();

    if (distanceToTarget <= enemy.AttackRange && canAttack())
    {
      enemy.StopMovement();
      enemy.ChangeState(new DragonBabyRangedAttackState(enemy));
    }
    else
    {
      enemy.MoveTo(enemy.CurrentTarget.position);
      enemy.LookAtTarget();
    }
  }

  bool canAttack()
  {
    return Time.time >= enemy.LastAttackTime + enemy.AttackCooldown;
  }

  public void Exit()
  {
    enemy.StopMovement();
    enemy.Animator.ToggleRun(false);
  }
}