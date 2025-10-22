using UnityEngine;

public class MiniDragonSleepingState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;

  public MiniDragonSleepingState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetBool("Sleeping", true);
    _boss.DisableMovementAndCollisions();
    _boss.Rb.isKinematic = true;
    _boss.Rb.useGravity = true;
    _boss.EnableCollisions();
  }

  public void Tick() { }

  public void OnExit()
  {
    _boss.Animator.SetBool("Sleeping", false);
  }
}