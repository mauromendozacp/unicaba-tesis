using UnityEngine;

public class DragonTransitionLandingState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;

  public DragonTransitionLandingState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("Lands");
    _boss.Rb.useGravity = true;
  }

  public void Tick()
  {
    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);
    if (stateInfo.IsName("Lands") && stateInfo.normalizedTime >= 0.75f)
    {
      // Opcional: Generar una onda expansiva de daño al aterrizar aquí.
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    _boss.Rb.linearVelocity = Vector3.zero;
  }
}