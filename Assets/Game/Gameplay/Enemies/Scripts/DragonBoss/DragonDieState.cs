using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class DragonDieState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;

  public DragonDieState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("Die");
    _boss.EnableCollisions();
    _boss.Rb.useGravity = true;
    //_boss.Rb.linearVelocity = Vector3.zero;
    _boss.StopMovement();
    //_boss.DisableMovementAndCollisions();
    _boss.StartCoroutine(DieCorroutine());
  }

  public void Tick() { }

  public void OnExit() { }

  private IEnumerator DieCorroutine()
  {
    yield return new WaitForSeconds(3f);
    _boss.Die();
  }
}