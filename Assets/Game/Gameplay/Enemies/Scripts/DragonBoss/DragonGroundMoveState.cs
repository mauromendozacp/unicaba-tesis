using UnityEngine;

public class DragonGroundMoveState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private float _minAttackDistance = 3f;
  private float _maxAttackDistance = 7f;

  public DragonGroundMoveState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetBool("Walk", true);
    _boss.Rb.useGravity = true;
  }

  public void Tick()
  {
    Transform target = _boss.CurrentTarget;
    if (target == null)
    {
      _boss.ChangeState(_factory.GroundIdle());
      return;
    }

    Vector3 targetPos = target.position;
    Vector3 currentPos = _boss.transform.position;

    // Ignoramos la altura del Dragón (Y) en el cálculo de la dirección en tierra
    Vector3 flatDirection = (new Vector3(targetPos.x, currentPos.y, targetPos.z) - currentPos).normalized;
    float distance = Vector3.Distance(currentPos, targetPos);

    // Rotación hacia el objetivo
    if (flatDirection.sqrMagnitude > 0.01f)
    {
      Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
      _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed);
    }

    // Lógica de Persecución/Posicionamiento
    if (distance < _minAttackDistance)
    {
      // Demasiado cerca, preparar mordida o alejarse un poco
      _boss.Rb.linearVelocity = Vector3.zero;
      _boss.ChangeState(_factory.GroundAttack("Bite"));
    }
    else if (distance > _maxAttackDistance)
    {
      // Muy lejos, acercarse
      _boss.Rb.linearVelocity = flatDirection * _boss.groundMoveSpeed;
      //Debug.Log($"DRAGÓN AVANZANDO hacia el objetivo. {distance}m de distancia. {_boss.Rb.linearVelocity} m/s");
    }
    else
    {
      // Distancia óptima, iniciar ataque a distancia o reposo
      _boss.Rb.linearVelocity = Vector3.zero;
      _boss.ChangeState(_factory.GroundAttack("Drakaris"));
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("Walk", false);
    _boss.Rb.linearVelocity = Vector3.zero;
  }
}