using UnityEngine;
using System.Collections;

public class SkeletonDeathState : IEnemyState
{
  private readonly SkeletonEnemy enemy;
  public EnemyState State { get; private set; }

  public SkeletonDeathState(SkeletonEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Death;
  }

  public void Enter()
  {
    enemy.DisableMovementAndCollisions();
    enemy.Animator?.TriggerDeath();
    enemy.StartCoroutine(DieCoroutine());
  }

  public void Update() { }

  public void Exit()
  {
  }

  private IEnumerator DieCoroutine()
  {
    yield return new WaitForSeconds(4f);
    enemy.Die();
  }
}