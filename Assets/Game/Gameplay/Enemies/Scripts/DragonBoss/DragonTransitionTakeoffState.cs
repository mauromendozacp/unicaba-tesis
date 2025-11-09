using UnityEngine;
public class DragonTransitionTakeoffState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private float _takeoffHeight = 10f; // Altura objetivo
  private float _heightTolerance = 1f;

  public DragonTransitionTakeoffState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.IsVulnerable = false;
    _boss.Animator.SetTrigger("TakeOff");
    _boss.Rb.useGravity = false;
    _boss.Rb.isKinematic = false;
    _boss.DisableMovementAndCollisions();
  }

  public void Tick()
  {
    Vector3 targetPos = new Vector3(_boss.transform.position.x, _takeoffHeight, _boss.transform.position.z);
    Vector3 direction = (targetPos - _boss.transform.position).normalized;
    _boss.Rb.linearVelocity = direction * _boss.airMoveSpeed;

    if (Vector3.Distance(_boss.transform.position, targetPos) < _heightTolerance)
    {
      HandleAnimationEnd();
    }
  }

  public void OnExit()
  {
    _boss.Rb.linearVelocity = Vector3.zero;
  }

  // Llamado desde el evento de animación o cuando se alcanza la altura
  public void HandleAnimationEnd()
  {
    _boss.ChangeState(_factory.AirFlyReposition());
  }
}