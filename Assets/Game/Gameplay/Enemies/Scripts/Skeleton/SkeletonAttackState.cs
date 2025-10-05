using System.Collections;
using UnityEngine;

public class SkeletonAttackState : IEnemyState
{
  private readonly SkeletonEnemy enemy;
  private float lastAttackTime;

  public EnemyState State { get; private set; }

  public SkeletonAttackState(SkeletonEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.StopMovement();
    lastAttackTime = Time.time;
    PerformAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new SkeletonIdleState(enemy));
      return;
    }

    enemy.LookAtTarget();

    if (enemy.CurrentTarget != null && enemy.DistanceToTarget() > enemy.AttackRange + 0.5f)
    {
      enemy.ChangeState(new SkeletonChaseState(enemy));
    }
    else if (Time.time >= lastAttackTime + enemy.AttackCooldown)
    {
      PerformAttack();
      lastAttackTime = Time.time;
    }
  }

  private void PerformAttack()
  {
    if (enemy.Animator != null)
    {
      enemy.Animator.TriggerAttack();
    }
  }


  public void Exit()
  {
  }
}