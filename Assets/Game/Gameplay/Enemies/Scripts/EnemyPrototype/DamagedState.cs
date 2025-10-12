// DamagedState.cs
using UnityEngine;
using System.Collections;

public class DamagedState : IEnemyState
{
  private readonly EnemyPrototype enemy;
  private const float knockbackForce = 5f;
  public EnemyState State { get; private set; }
  private const float recoveryTime = 0.15f;

  public DamagedState(EnemyPrototype enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    //enemy.ApplyKnockbackForce();
    enemy.ToggleDamageMaterial(true);
    //enemy.ChangeMaterial();
    enemy.StartCoroutine(ReturnToPreviousState());
  }

  public void Update() { }

  public void Exit()
  {
    //enemy.ResetKnockbackForce();
    enemy.ToggleDamageMaterial(false);
  }

  private IEnumerator ReturnToPreviousState()
  {
    yield return new WaitForSeconds(recoveryTime);

    if (!enemy.IsAlive)
    {
      enemy.ChangeState(new DeathState(enemy));
      yield return null;
    }

    /*if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new ChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new IdleState(enemy));
    }
    */
    enemy.ChangeState(new IdleState(enemy));
  }
}

