using UnityEngine;
using System.Collections;

public class DragonBabyRangedDamagedState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedDamagedState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Damaged;
  }

  public void Enter()
  {
    //enemy.ApplyKnockbackForce();
    enemy.ToggleDamageMaterial(true);
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
    yield return new WaitForSeconds(0.1f);

    if (!enemy.IsAlive)
    {
      enemy.ChangeState(new DragonBabyRangedDeathState(enemy));
      yield break;
    }

    if (enemy.CurrentTarget != null)
    {
      enemy.ChangeState(new DragonBabyRangedChaseState(enemy));
    }
    else
    {
      enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
    }
  }
}