// AttackState.cs
using UnityEngine;
using System.Collections;

public class Prototype2AttackState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  private float lastAttackTime;
  public EnemyState State { get; private set; }

  public Prototype2AttackState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.ChangeMaterial();
    enemy.StopAllCoroutines(); // Detener cualquier coroutine previa, como la de retroceso
    lastAttackTime = Time.time;
    PerformAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.CurrentTarget.GetComponent<IDamageable>().IsAlive)
    {
      enemy.ChangeState(new Prototype2IdleState(enemy));
      return;
    }

    enemy.transform.LookAt(enemy.CurrentTarget);

    if (Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position) > enemy.AttackDistance)
    {
      enemy.ChangeState(new Prototype2ChaseState(enemy));
    }
    else if (Time.time >= lastAttackTime + enemy.AttackCooldown)
    {
      PerformAttack();
      enemy.ChangeState(new Prototype2RetreatState(enemy)); // Después de atacar, retrocede
    }
  }

  private void PerformAttack()
  {
    enemy.ShootFireBall();
  }

  public void Exit()
  {
    enemy.ActiveMovement();
  }
}