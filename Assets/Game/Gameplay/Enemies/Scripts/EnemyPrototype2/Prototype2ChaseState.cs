// ChaseState.cs
using UnityEngine;

public class Prototype2ChaseState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  public EnemyState State { get; private set; }

  public Prototype2ChaseState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Chase;
  }

  public void Enter()
  {
    enemy.ChangeMaterial();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null)
    {
      enemy.ChangeState(new Prototype2IdleState(enemy));
      return;
    }

    float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position);

    // Si ya está a la distancia de ataque, cambia a AttackState
    if (distanceToTarget <= enemy.AttackDistance)
    {
      enemy.ChangeState(new Prototype2AttackState(enemy));
    }
    // Si no, se mueve hacia el jugador
    else
    {
      enemy.transform.LookAt(enemy.CurrentTarget);
      enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.CurrentTarget.position, enemy.CurrentSpeed * Time.deltaTime);
    }
  }

  public void Exit()
  {
  }
}