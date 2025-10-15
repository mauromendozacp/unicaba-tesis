using UnityEngine;

public class DragonAirFlyRepositionState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private Vector3 _targetPosition;
  private float _repositionTime = 3f;
  private float _timer;

  public DragonAirFlyRepositionState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    //_boss.animator.Play("flying FWD");
    _boss.Animator.SetBool("FlyingFWD", true);
    _timer = _repositionTime;

    // Elegir una posición aleatoria y alta en la arena para reposicionarse
    _targetPosition = new Vector3(Random.Range(-20f, 20f), 10f, Random.Range(-20f, 20f));
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;

    // Mover y rotar hacia el objetivo en el aire
    Vector3 direction = (_targetPosition - _boss.transform.position).normalized;
    _boss.Rb.linearVelocity = direction * _boss.airMoveSpeed;

    Quaternion lookRotation = Quaternion.LookRotation(direction);
    _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed);

    if (_timer <= 0)
    {
      // Tras reposicionarse, ataca o aterriza
      if (Random.value > 0.6f)
      {
        _boss.ChangeState(_factory.TransitionLanding());
      }
      else
      {
        _boss.ChangeState(_factory.AirHoverAttack());
      }
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("FlyingFWD", false);
    _boss.Rb.linearVelocity = Vector3.zero;
  }
}