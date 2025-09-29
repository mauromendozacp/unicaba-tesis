using UnityEngine;

public class Prototype2IdleState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  public EnemyState State { get; private set; }

  public Prototype2IdleState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Idle;
  }

  public void Enter()
  {
    enemy.ChangeMaterial();
  }

  public void Update()
  {
    Collider[] players = Physics.OverlapSphere(enemy.transform.position, enemy.ChaseRadius, LayerMask.GetMask("Player"));
    if (players.Length > 0)
    {
      Transform closestTarget = null;
      float minDistance = Mathf.Infinity;

      foreach (var playerCollider in players)
      {
        IDamageable damageable = playerCollider.GetComponent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) continue;
        float dist = Vector3.Distance(enemy.transform.position, playerCollider.transform.position);
        if (dist < minDistance)
        {
          minDistance = dist;
          closestTarget = playerCollider.transform;
        }
      }
      enemy.CurrentTarget = closestTarget;
      enemy.ChangeState(new Prototype2ChaseState(enemy));
    }
  }

  public void Exit()
  {
  }
}