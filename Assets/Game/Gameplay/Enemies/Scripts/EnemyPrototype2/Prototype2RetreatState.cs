// RetreatState.cs
using UnityEngine;
using System.Collections;

public class Prototype2RetreatState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  private Vector3 retreatDirection;
  public EnemyState State { get; private set; }

  private const float retreatDuration = 2f;

  public Prototype2RetreatState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Retreat;
  }

  public void Enter()
  {
    // Elegir una dirección aleatoria para retroceder
    retreatDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    enemy.StartCoroutine(RetreatCoroutine());
  }

  public void Update()
  {
    // Se mueve en la dirección de retroceso
    enemy.transform.position += retreatDirection * enemy.CurrentSpeed * Time.deltaTime;
  }

  public void Exit()
  {
  }

  private IEnumerator RetreatCoroutine()
  {
    yield return new WaitForSeconds(retreatDuration);
    enemy.ChangeState(new Prototype2ChaseState(enemy));
  }
}