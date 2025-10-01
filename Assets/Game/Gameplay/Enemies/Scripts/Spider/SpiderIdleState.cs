using UnityEngine;

public class SpiderIdleState : IEnemyState
{
  private readonly SpiderEnemy enemy;
  public EnemyState State { get; private set; }

  public SpiderIdleState(SpiderEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Idle;
  }

  public void Enter()
  {
    if (enemy.Animator != null)
    {
      enemy.Animator.ToggleWalk(false);
      enemy.Animator.ToggleIdle(true);
    }
    enemy.StopMovement();
  }

  public void Update()
  {
    enemy.CurrentTarget = enemy.FindNearestPlayer();
    if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new SpiderChaseState(enemy));
    }
  }

  public void Exit()
  {
    enemy.Animator.ToggleIdle(false);
  }
}