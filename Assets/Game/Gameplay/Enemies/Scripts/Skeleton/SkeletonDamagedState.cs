using UnityEngine;
using System.Collections;

public class SkeletonDamagedState : IEnemyState
{
  private readonly SkeletonEnemy enemy;

  public EnemyState State { get; private set; }
  private const float recoveryTime = 1.5f;

  public SkeletonDamagedState(SkeletonEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    enemy.Animator?.TriggerDamage();
    enemy.DisableMovementAndCollisions();
    enemy.StartCoroutine(ReturnToPreviousState());
  }

  public void Update() { }

  public void Exit()
  {
    enemy.EnableMovementAndCollisions();
  }

  private IEnumerator ReturnToPreviousState()
  {
    yield return new WaitForSeconds(recoveryTime);

    if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new SkeletonChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new SkeletonIdleState(enemy));
    }
  }
}