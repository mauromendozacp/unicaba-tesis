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
    //enemy.DisableMovementAndCollisions();
    enemy.Animator.TriggerDeath();
    // Opcional: animar o rotar para simular una caída
    // enemy.transform.rotation = Quaternion.Euler(90, enemy.transform.rotation.y, enemy.transform.rotation.z);

    enemy.StartCoroutine(DieCoroutine());
  }

  public void Update() { }

  public void Exit() { }

  private IEnumerator DieCoroutine()
  {
    yield return new WaitForSeconds(3f);
    enemy.Die();
  }
}