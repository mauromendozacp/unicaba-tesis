using UnityEngine;

public class SpiderChaseState : IEnemyState
{
  private readonly SpiderEnemy enemy;
  public EnemyState State { get; private set; }

  public SpiderChaseState(SpiderEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Chase;
  }

  public void Enter()
  {
    if (enemy.Animator != null)
    {
      enemy.Animator.ToggleWalk(true);
    }
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new SpiderIdleState(enemy));
      return;
    }

    if (enemy.IsTooFarFromOrigin())
    {
      enemy.StopMovement();
      enemy.ChangeState(new SpiderIdleState(enemy));
      return;
    }

    if (enemy.CurrentTarget != null && enemy.DistanceToTarget() <= enemy.AttackRange + 0.1f)
    {
      enemy.ChangeState(new SpiderAttackState(enemy));
      return;
    }

    enemy.MoveTo(enemy.CurrentTarget.position);
  }

  public void Exit()
  {
    if (enemy.Animator != null)
    {
      enemy.Animator.ToggleWalk(false);
    }
    enemy.StopMovement();
  }
}