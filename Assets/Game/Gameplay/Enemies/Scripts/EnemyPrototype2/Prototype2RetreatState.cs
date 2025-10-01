// RetreatState.cs
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Prototype2RetreatState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  private Vector3 retreatDirection;
  public EnemyState State { get; private set; }
  private const float retreatDistance = 3f;
  private const float retreatDuration = 2f;

  public Prototype2RetreatState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Retreat;
  }

  public void Enter()
  {
    if (enemy.CurrentTarget != null)
    {
      // Dirección lejos del objetivo
      Vector3 runAwayDirection = (enemy.transform.position - enemy.CurrentTarget.position).normalized;
      Vector3 destination = enemy.transform.position + runAwayDirection * retreatDistance;

      // Intentar encontrar el punto más cercano en el NavMesh
      if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
      {
        enemy.MoveTo(hit.position); // Mover el agente al punto seguro
      }
      else
      {
        // Si no encuentra un punto válido, solo espera y luego cambia de estado
        Debug.LogWarning("No se encontró un punto de retroceso válido en el NavMesh.");
      }
    }
    // Elegir una dirección aleatoria para retroceder
    //retreatDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    enemy.StartCoroutine(RetreatCoroutine());
  }

  public void Update()
  {
    // Se mueve en la dirección de retroceso
    //enemy.transform.position += retreatDirection * enemy.CurrentSpeed * Time.deltaTime;
    // El NavMeshAgent se encarga del movimiento.
    // Opcional: Podrías añadir lógica aquí para forzar una rotación si el NavMeshAgent no la está manejando bien.
  }

  public void Exit()
  {
    enemy.StopMovement();
  }

  private IEnumerator RetreatCoroutine()
  {
    yield return new WaitForSeconds(retreatDuration);
    //enemy.ChangeState(new Prototype2ChaseState(enemy));
    enemy.ChangeState(new Prototype2IdleState(enemy));
  }
}