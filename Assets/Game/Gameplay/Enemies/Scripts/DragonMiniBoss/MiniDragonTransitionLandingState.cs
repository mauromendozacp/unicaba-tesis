using UnityEngine;

public class MiniDragonTransitionLandingState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;

  public MiniDragonTransitionLandingState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("Land");
    _boss.EnableCollisions();
    _boss.Rb.useGravity = true;
    _boss.Rb.isKinematic = true;
  }

  public void Tick()
  {
    // Verificar si la animación de aterrizaje ha terminado.
    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);

    // Usamos 0.75f como punto de referencia para considerar el aterrizaje visualmente completado.
    if (stateInfo.IsName("Land") && stateInfo.normalizedTime >= 0.75f)
    {
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    _boss.Rb.useGravity = false;
    _boss.Rb.isKinematic = false;
    _boss.Rb.linearVelocity = Vector3.zero;
    _boss.Rb.isKinematic = true;
    _boss.Rb.useGravity = true;
    _boss.EnableMovementAndCollisions();
  }
}