// DeathState.cs (modificado para EnemyPrototype2)
using UnityEngine;
using System.Collections;

public class Prototype2DeathState : IEnemyState
{
  private readonly EnemyPrototype2 enemy;
  public EnemyState State { get; private set; }

  public Prototype2DeathState(EnemyPrototype2 enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Death;
  }

  public void Enter()
  {
    enemy.transform.rotation = Quaternion.Euler(90, enemy.transform.rotation.y, enemy.transform.rotation.z);
    enemy.GetComponent<Collider>().enabled = false;
    enemy.StartCoroutine(DieCoroutine());
  }

  public void Update() { }

  public void Exit()
  {
  }

  private IEnumerator DieCoroutine()
  {
    yield return new WaitForSeconds(3f);
    enemy.Die();
  }
}