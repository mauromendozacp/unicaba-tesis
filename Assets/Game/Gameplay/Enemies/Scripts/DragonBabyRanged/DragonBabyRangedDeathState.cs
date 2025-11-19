using UnityEngine;
using System.Collections;

public class DragonBabyRangedDeathState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedDeathState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Death;
  }

  public void Enter()
  {
    enemy.StopMovement();

    /* CORREGIR */
    //enemy.DisableMovementAndCollisions();
    enemy.StopAllCoroutines();
    enemy.SetAttackCollider(false);

    enemy.Animator.TriggerDeath();

    enemy.StartCoroutine(DieCoroutine());
  }

  public void Update() { }

  public void Exit() { }

  private IEnumerator DieCoroutine()
  {
    enemy.ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.1f);
    enemy.ToggleDamageMaterial(false);
    //enemy.ToggleDamageMaterial(true);
    yield return new WaitForSeconds(3f);
    enemy.Die();
  }
}