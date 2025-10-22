using UnityEngine;

public class MiniDragonAirRepositionState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private Vector3 _targetPosition;
  private float _repositionTime = 3f;
  private float _timer;
  private float _zigzagFrequency = 2f; // Frecuencia del cambio de dirección
  private float _zigzagTimer;

  // Variables para el movimiento en Zigzag
  private Vector3 _baseDirection;
  private float _zigzagMagnitude = 0.5f;

  public MiniDragonAirRepositionState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    if (_boss.CurrentTarget == null)
    {
      _boss.ChangeState(_factory.TransitionLanding());
      return;
    }
    _boss.Animator.SetBool("FlyForward", true);
    _timer = _repositionTime;
    _zigzagTimer = _zigzagFrequency;

    // 1. Elegir un punto de reposicionamiento (altura baja)
    _baseDirection = (_boss.CurrentTarget.position - _boss.transform.position).normalized;

    // Simplemente vuela hacia el target por 3s, o hasta que aterrice
    _targetPosition = new Vector3(_boss.CurrentTarget.position.x, _boss.FlyHeight, _boss.CurrentTarget.position.z);
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;
    _zigzagTimer -= Time.deltaTime;

    // 1. Lógica de Zigzag
    if (_zigzagTimer <= 0)
    {
      // Cambiar la dirección lateralmente (ej. rotar 30 grados a izquierda o derecha)
      float angle = Random.Range(30f, 60f) * (Random.value > 0.5f ? 1 : -1);
      _baseDirection = Quaternion.Euler(0, angle, 0) * _baseDirection;
      _zigzagTimer = _zigzagFrequency;
    }

    // 2. Aplicar movimiento
    Vector3 finalDirection = (_baseDirection + _boss.transform.right * Mathf.Sin(Time.time * 5f) * _zigzagMagnitude).normalized;
    _boss.Rb.linearVelocity = finalDirection * _boss.airMoveSpeed;

    // 3. Rotación
    Quaternion lookRotation = Quaternion.LookRotation(_baseDirection);
    _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed);

    // 4. Decisión de Transición
    if (_timer <= 0 || _boss.DistanceToTarget() < 4f)
    {
      // Forzar aterrizaje al llegar cerca
      _boss.ChangeState(_factory.TransitionLanding());
    }
    else if (Random.value < 0.2f && _timer < _repositionTime - 1.0f)
    {
      // Opcional: Atacar a distancia desde el aire
      _boss.ChangeState(_factory.AirRangedAttack());
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("FlyForward", false);
    _boss.Rb.linearVelocity = Vector3.zero;
    _boss.Rb.angularVelocity = Vector3.zero;
  }
}