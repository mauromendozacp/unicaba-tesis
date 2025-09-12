// DamagedState.cs
using UnityEngine;
using System.Collections;

public class DamagedState : IEnemyState
{
  private readonly EnemyPrototype enemy;
  private const float knockbackForce = 5f;
  public EnemyState State { get; private set; }

  public DamagedState(EnemyPrototype enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    enemy.ApplyKnockbackForce();
    enemy.StartCoroutine(ReturnToPreviousState());
  }

  public void Update() { }

  public void Exit()
  {
    enemy.ResetKnockbackForce();
  }

  private IEnumerator ReturnToPreviousState()
  {
    yield return new WaitForSeconds(1f);
    if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new ChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new IdleState(enemy));
    }
  }
}

