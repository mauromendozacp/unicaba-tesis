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
    lastAttackTime = Time.time;
    PerformAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null)
    {
      enemy.ChangeState(new SpiderIdleState(enemy));
      return;
    }

    // Rotar para mirar al objetivo (incluso durante el ataque)
    //enemy.transform.LookAt(enemy.CurrentTarget);

    if (enemy.CurrentTarget != null && enemy.DistanceToTarget() > enemy.AttackRange + 0.5f) // Agregamos un pequeño margen para que no ataque desde muy lejos
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
    // Activar animación de ataque
    if (enemy.Animator != null)
    {
      enemy.Animator.TriggerAttack();
    }

    // Activa el collider del ataque durante un breve periodo (depende del tiempo de la animación)
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