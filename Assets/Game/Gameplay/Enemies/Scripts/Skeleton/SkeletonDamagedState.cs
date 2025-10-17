using UnityEngine;
using System.Collections;

public class SkeletonDamagedState : IEnemyState
{
  private readonly SkeletonEnemy enemy;

  public EnemyState State { get; private set; }
  private const float recoveryTime = 0.25f;

  public SkeletonDamagedState(SkeletonEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    //enemy.Animator?.TriggerDamage();
    enemy.ToggleDamageMaterial(true);
    //enemy.DisableMovementAndCollisions();
    enemy.ToggleSelfCollider(false);
    enemy.StartCoroutine(ReturnToPreviousState());
  }

  public void Update() { }

  public void Exit()
  {
    //enemy.EnableMovementAndCollisions();
    enemy.ToggleSelfCollider(true);
    enemy.ToggleDamageMaterial(false);
  }

  private IEnumerator ReturnToPreviousState()
  {
    yield return new WaitForSeconds(recoveryTime);

    if (!enemy.IsAlive)
    {
      enemy.ChangeState(new SkeletonDeathState(enemy));
      yield return null;
    }

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