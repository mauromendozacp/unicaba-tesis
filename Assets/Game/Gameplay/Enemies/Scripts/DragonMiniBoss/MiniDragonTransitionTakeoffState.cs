using UnityEngine;

public class MiniDragonTransitionTakeoffState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private float _heightTolerance = 0.25f;

  public MiniDragonTransitionTakeoffState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("TakeOff");
    _boss.Rb.useGravity = false;
    _boss.DisableMovementAndCollisions();
    _boss.Rb.isKinematic = false;
  }

  public void Tick()
  {
    // Mover hacia la altura de vuelo definida en el controlador
    float targetHeight = _boss.FlyHeight;

    Vector3 targetPos = new Vector3(_boss.transform.position.x, targetHeight, _boss.transform.position.z);
    Vector3 direction = (targetPos - _boss.transform.position).normalized;

    _boss.Rb.linearVelocity = direction * _boss.airMoveSpeed;

    if (Vector3.Distance(_boss.transform.position, targetPos) < _heightTolerance)
    {
      HandleAnimationEnd();
    }

    // Nota: Para mejorar, podrías esperar a que la animación de Talk Off haya progresado
    // antes de cambiar de estado, usando Animation Events o normalizedTime.
  }

  public void OnExit()
  {
    _boss.Rb.linearVelocity = Vector3.zero;
  }

  public void HandleAnimationEnd()
  {
    _boss.ChangeState(_factory.AirReposition());
  }
}