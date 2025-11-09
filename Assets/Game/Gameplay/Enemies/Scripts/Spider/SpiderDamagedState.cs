using UnityEngine;
using System.Collections;

public class SpiderDamagedState : IEnemyState
{
  private readonly SpiderEnemy enemy;

  public EnemyState State { get; private set; }
  private const float recoveryTime = 0.25f;

  public SpiderDamagedState(SpiderEnemy enemy)
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
      enemy.ChangeState(new SpiderDeathState(enemy));
      yield return null;
    }

    if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new SpiderChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new SpiderIdleState(enemy));
    }
  }
}