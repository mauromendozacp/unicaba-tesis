using System.Collections;
using UnityEngine;

public class SpiderAttackState : IEnemyState
{
  private readonly SpiderEnemy enemy;
  private float lastAttackTime;

  public EnemyState State { get; private set; }

  public SpiderAttackState(SpiderEnemy enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.StopMovement();
    lastAttackTime = Time.time;
    PerformAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new SpiderIdleState(enemy));
      return;
    }

    enemy.LookAtTarget();

    if (enemy.CurrentTarget != null && enemy.DistanceToTarget() > enemy.AttackRange + 0.5f)
    {
      enemy.ChangeState(new SpiderChaseState(enemy));
    }
    else if (Time.time >= lastAttackTime + enemy.AttackCooldown)
    {
      PerformAttack();
      lastAttackTime = Time.time;
    }
  }

  private void PerformAttack()
  {
    if (enemy.Animator != null)
    {
      enemy.Animator.TriggerAttack();
    }
    enemy.StartCoroutine(ActivateAttackColliderForTime(0.5f));
  }

  private IEnumerator ActivateAttackColliderForTime(float duration)
  {
    enemy.SetAttackCollider(true);
    yield return new WaitForSeconds(duration);
    enemy.SetAttackCollider(false);
  }

  public void Exit()
  {
    enemy.SetAttackCollider(false);
  }
}