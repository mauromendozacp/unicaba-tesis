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
    _boss.Animator.SetBool("FlyingFWD", true);
    _timer = _repositionTime;

    // Elegir una posición aleatoria y alta en la arena para reposicionarse
    Vector3 currentPosition = _boss.transform.position;
    Vector3 newTargetPosition;
    float minDistanceSquared = 15f * 15f;
    float distanceSquared;

    // Bucle para elegir una posición aleatoria que cumpla con el requisito de distancia
    do
    {
      // Elegir una posición aleatoria dentro del rango -20 a 20 para x y z.
      float randomX = Random.Range(-20f, 20f);
      float randomZ = Random.Range(-20f, 20f);
      newTargetPosition = new Vector3(randomX, 10f, randomZ);

      float dx = newTargetPosition.x - currentPosition.x;
      float dz = newTargetPosition.z - currentPosition.z;
      distanceSquared = dx * dx + dz * dz;
    } while (distanceSquared < minDistanceSquared);

    _targetPosition = newTargetPosition;

    /* Debug */
    _boss.CurrentRepositionTarget = _targetPosition;
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;

    Vector3 direction = (_targetPosition - _boss.transform.position).normalized;
    _boss.Rb.linearVelocity = direction * _boss.airMoveSpeed;

    Quaternion lookRotation = Quaternion.LookRotation(direction);
    _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed / 10);

    if (_timer <= 0)
    {
      if(_boss.CurrentTarget == null)
      {
        _boss.ChangeState(_factory.TransitionLanding());
      }
      else if ( Random.value > 0.6f)
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

    /* Debug */
    _boss.CurrentRepositionTarget = Vector3.zero;
  }
}