using UnityEngine;

public class DragonBabyRangedChaseState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedChaseState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Chase;
  }

  public void Enter()
  {
    // Establecer la distancia de parada del NavMeshAgent a la distancia óptima de ataque
    //enemy.Agent.stoppingDistance = enemy.OptimalAttackDistance;
    enemy.Animator.ToggleRun(true);
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
      return;
    }

    float distanceToTarget = enemy.DistanceToTarget();

    // 1. Si está dentro del rango óptimo, cambia a AttackState
    if (distanceToTarget <= enemy.OptimalAttackDistance)
    {
      enemy.StopMovement();
      enemy.ChangeState(new DragonBabyRangedAttackState(enemy));
    }
    // 2. Si está fuera de rango, persigue
    else
    {
      enemy.MoveTo(enemy.CurrentTarget.position);
      enemy.LookAtTarget();
    }
  }

  public void Exit()
  {
    enemy.StopMovement();
    enemy.Animator.ToggleRun(false);
    // Reestablecer la distancia de parada para el movimiento general
    //enemy.Agent.stoppingDistance = 0.1f;
  }
}