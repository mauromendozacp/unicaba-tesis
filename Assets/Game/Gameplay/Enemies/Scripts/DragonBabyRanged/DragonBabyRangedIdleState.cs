using UnityEngine;

public class DragonBabyRangedIdleState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedIdleState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Idle;
  }

  public void Enter()
  {
    enemy.StopMovement();
    enemy.Animator.ToggleIdle(true);
  }

  public void Update()
  {
    Transform closestTarget = enemy.FindNearestPlayer();

    if (closestTarget != null)
    {
      enemy.CurrentTarget = closestTarget;
      enemy.ChangeState(new DragonBabyRangedChaseState(enemy));
    }
  }

  public void Exit()
  {
    enemy.Animator.ToggleIdle(false);
  }
}