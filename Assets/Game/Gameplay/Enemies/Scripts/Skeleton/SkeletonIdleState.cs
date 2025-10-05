using UnityEngine;

public class SkeletonIdleState : IEnemyState
{
  private readonly SkeletonEnemy enemy;
  public EnemyState State { get; private set; }

  public SkeletonIdleState(SkeletonEnemy enemy)
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

    if (enemy.CurrentTarget != null && !enemy.IsTooFarFromOrigin())
    {
      enemy.ChangeState(new SkeletonChaseState(enemy));
    }

    if (enemy.IsTerritorial)
    {
      Vector3 initialPos = enemy.GetInitialPosition();
      float distanceToInitial = Vector3.Distance(enemy.transform.position, initialPos);

      if (distanceToInitial > 0.5f)
      {
        if (enemy.Animator != null)
        {
          enemy.Animator.ToggleWalk(true);
          enemy.Animator.ToggleIdle(false);
        }
        enemy.MoveTo(initialPos);
      }
      else
      {
        if (enemy.Animator != null)
        {
          enemy.Animator.ToggleWalk(false);
          enemy.Animator.ToggleIdle(true);
        }
        enemy.StopMovement();
      }
    }
    else
    {
      enemy.StopMovement();
    }
  }

  public void Exit()
  {
    enemy.Animator.ToggleIdle(false);
  }
}