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
      enemy.ChangeState(new IdleState(enemy));
      return;
    }

    enemy.transform.LookAt(enemy.CurrentTarget);
    enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.CurrentTarget.position, enemy.CurrentSpeed * Time.deltaTime);

    if (Vector3.Distance(enemy.transform.position, enemy.CurrentTarget.position) <= enemy.AttackRange)
    {
      enemy.ChangeState(new AttackState(enemy));
    }
  }

  public void Exit()
  {

  }
}
