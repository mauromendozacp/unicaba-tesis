using UnityEngine;

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
    _boss.Rb.linearVelocity = Vector3.zero;
    _boss.Rb.useGravity = true;

    _boss.Die();
  }

  public void Tick() { }

  public void OnExit() { }
}