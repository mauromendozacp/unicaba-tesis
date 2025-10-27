using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class DragonBabyRangedRetreatState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }
  private const float retreatDistance = 6f;
  private const float retreatDuration = 3f;
  private const float maxRetreatAngle = 45f;

  public DragonBabyRangedRetreatState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Retreat;
  }

  public void Enter()
  {
    enemy.Animator.ToggleRetreat(true);
    if (enemy.CurrentTarget != null)
    {
      // 1. Calcular punto de retroceso (lejos del objetivo)
      Vector3 straightBackDirection = (enemy.transform.position - enemy.CurrentTarget.position).normalized;
      float randomAngle = Random.Range(-maxRetreatAngle, maxRetreatAngle);
      Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);
      Vector3 runAwayDirection = randomRotation * straightBackDirection;
      Vector3 destination = enemy.transform.position + runAwayDirection * retreatDistance;

      // 2. Intentar encontrar el punto más cercano en el NavMesh
      if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
      {
        enemy.MoveTo(hit.position);
      }
      else
      {
        // Si no encuentra punto válido, solo espera el tiempo
        //enemy.StopMovement();
        enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
        return;
      }
    }
    enemy.StartCoroutine(RetreatCoroutine());
  }

  public void Update()
  {
    // Asegurar que siga moviéndose hacia el punto de retroceso
    enemy.LookAtTarget();
  }

  public void Exit()
  {
    enemy.StopMovement();
    enemy.Animator.ToggleRetreat(false);
  }

  private IEnumerator RetreatCoroutine()
  {
    yield return new WaitForSeconds(retreatDuration);
    // Vuelve a Chase para reevaluar la posición de ataque
    enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
  }
}
