// ChaseState.cs
using UnityEngine;

public class ChaseState : IEnemyState
{
  private readonly EnemyPrototype enemy;
  public EnemyState State { get; private set; }

  public ChaseState(EnemyPrototype enemy)
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
      //enemy.StopMovement(); // <--- Detener movimiento al ir a Idle
      enemy.ChangeState(new IdleState(enemy));
      return;
    }

    //enemy.transform.LookAt(enemy.CurrentTarget);
    //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.CurrentTarget.position, enemy.CurrentSpeed * Time.deltaTime);
    // El agente manejará la rotación y el movimiento
    enemy.MoveTo(enemy.CurrentTarget.position); // <--- Usar NavMeshAgent

    if (Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position) <= enemy.AttackRange)
    {
      //enemy.StopMovement(); // <--- Detener movimiento para el ataque
      enemy.ChangeState(new AttackState(enemy));
    }
  }

  public void Exit()
  {
    enemy.StopMovement();
  }
}
