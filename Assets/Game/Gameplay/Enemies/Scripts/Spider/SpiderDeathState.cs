using UnityEngine;
using System.Collections;

public class SpiderDeathState : IEnemyState
{
  private readonly SpiderEnemy enemy;
  public EnemyState State { get; private set; }

  public SpiderDeathState(SpiderEnemy enemy)
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
    yield return new WaitForSeconds(3f);
    enemy.Die();
  }
}