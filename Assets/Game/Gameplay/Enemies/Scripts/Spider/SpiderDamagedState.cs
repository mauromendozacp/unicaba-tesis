using UnityEngine;
using System.Collections;

public class SpiderDamagedState : IEnemyState
{
  private readonly SpiderEnemy enemy;

  private const float knockbackForce = 1f;
  public EnemyState State { get; private set; }
  private const float recoveryTime = 1f; // Tiempo que pasa aturdido

  public SpiderDamagedState(SpiderEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    enemy.Animator?.TriggerDamage();
    //enemy.ApplyKnockbackForce();
    enemy.DisableMovementAndCollisions();
    enemy.StartCoroutine(ReturnToPreviousState());
  }

  public void Update() { }

  public void Exit()
  {
    //enemy.ResetKnockbackForce();
    enemy.EnableMovementAndCollisions();
  }

  private IEnumerator ReturnToPreviousState()
  {
    yield return new WaitForSeconds(recoveryTime);

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