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
    enemy.ActiveMovement();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null)
    {
      //enemy.StopMovement(); // <--- Detener movimiento al ir a Idle
      enemy.ChangeState(new Prototype2IdleState(enemy));
      return;
    }

    float distanceToTarget = Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position);

    // Si ya está a la distancia de ataque, cambia a AttackState
    if (distanceToTarget <= enemy.AttackDistance)
    {
      //enemy.StopMovement(); // <--- Detener movimiento para el ataque
      enemy.ChangeState(new Prototype2AttackState(enemy));
    }
    // Si no, usa NavMeshAgent para moverse hacia el jugador
    else
    {
      //enemy.transform.LookAt(enemy.CurrentTarget);
      //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.CurrentTarget.position, enemy.CurrentSpeed * Time.deltaTime);
      // El agente manejará la rotación y el movimiento
      enemy.MoveTo(enemy.CurrentTarget.position); // <--- Usar NavMeshAgent
    }
  }

  public void Exit()
  {
    enemy.StopMovement();
  }
}