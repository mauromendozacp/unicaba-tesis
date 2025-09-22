// DamagedState.cs (modificado para EnemyPrototype2)
using UnityEngine;
using System.Collections;

public class Prototype2DamagedState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  public EnemyState State { get; private set; }

  public Prototype2DamagedState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    enemy.ApplyKnockbackForce();
    enemy.ChangeMaterial();
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
      enemy.ChangeState(new Prototype2ChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new Prototype2IdleState(enemy));
    }
  }
}

