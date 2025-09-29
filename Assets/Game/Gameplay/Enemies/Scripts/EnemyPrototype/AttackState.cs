// AttackState.cs
using System.Collections;
using UnityEngine;

public class AttackState : IEnemyState
{
  private readonly EnemyPrototype enemy;
  private float lastAttackTime;

  public EnemyState State { get; private set; }

  public AttackState(EnemyPrototype enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.ChangeMaterial();
    lastAttackTime = Time.time;
    PerformAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.CurrentTarget.GetComponent<IDamageable>().IsAlive)
    {
      enemy.ChangeState(new IdleState(enemy));
      return;
    }


    if (Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position) > enemy.AttackRange)
    {
      enemy.ChangeState(new ChaseState(enemy));
    }
    else if (Time.time >= lastAttackTime + enemy.AttackCooldown)
    {
      PerformAttack();
      lastAttackTime = Time.time;
    }
  }

  private void PerformAttack()
  {
    enemy.StartCoroutine(ActivateAttackColliderForTime(0.5f)); // Activa el collider por 0.5 segundos
  }

  private IEnumerator ActivateAttackColliderForTime(float duration)
  {
    enemy.SetAttackCollider(true);
    yield return new WaitForSeconds(duration);
    enemy.SetAttackCollider(false);
  }

  public void Exit()
  {
    enemy.ActiveMovement();
    enemy.SetAttackCollider(false);
  }
}
