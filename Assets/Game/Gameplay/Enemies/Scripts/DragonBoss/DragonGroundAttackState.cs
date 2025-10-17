using UnityEngine;

public class DragonGroundAttackState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private string _attackAnimation;
  private bool _attackExecuted = false;
  float _attackRotationSpeed;
  bool _isFireballAttack = false;

  private Collider _damageTrigger;

  public DragonGroundAttackState(DragonBossController boss, DragonStateFactory factory, string animName)
  {
    _boss = boss;
    _factory = factory;
    _attackAnimation = animName;
    _attackRotationSpeed = _boss.AttackRotationSpeed;
    //_damageTrigger = animName == "Drakaris" ? _boss.FlameCollider : _boss.BiteCollider;

    if (animName == "Drakaris" || animName == "Bite")
    {
      _damageTrigger = animName == "Drakaris" ? _boss.FlameCollider : _boss.BiteCollider;
      _isFireballAttack = false;
    }
    else if (animName == "FireBall")
    {
      _isFireballAttack = true;
      _attackAnimation = "Drakaris";
      _damageTrigger = null;
    }
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger(_attackAnimation);
    //Debug.Log($"#{_boss.stateChangeCounter} Trigger {_attackAnimation} activado");
    _boss.Rb.linearVelocity = Vector3.zero;
    _attackExecuted = false;
  }

  public void Tick()
  {
    Transform target = _boss.CurrentTarget;
    if (target == null)
    {
      _boss.ChangeState(_factory.GroundIdle());
      return;
    }
    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);

    Vector3 flatDirection = (new Vector3(target.position.x, _boss.transform.position.y, target.position.z) - _boss.transform.position).normalized;
    Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
    _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _attackRotationSpeed);

    if (!_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.3f)
    {
      //Debug.Log("Ejecutando ataque: " + _attackAnimation);
      if (_isFireballAttack)
      {
        //Debug.Log("Lanzando ataque de bolas de fuego");
        ExecuteFireballAttack(target.position);
        //_boss.flame.SetActive(true);
        _attackExecuted = true;
      }
      else
      {
        //Debug.Log("Activando trigger de daño para: " + _attackAnimation);
        _damageTrigger.enabled = true;
        if (_attackAnimation == "Drakaris") _boss.flame.SetActive(true);
        _attackExecuted = true;
      }
    }
    if (_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.7f)
    {
      // Si la animación termina, regresa a Reposo para elegir la siguiente acción.
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    if (_damageTrigger != null && !_isFireballAttack) _damageTrigger.enabled = false;
    if (_attackAnimation == "Drakaris") _boss.flame.SetActive(false);
  }

  // Este método sería llamado desde DragonBossController.OnAnimationEvent(string eventName)
  public void HandleAttackEvent(string eventName)
  {
    // Ejemplo de uso de Animation Event:
    // if (eventName == "ActivateDamage") _damageTrigger.enabled = true;
    // if (eventName == "DeactivateDamage") _damageTrigger.enabled = false;
  }

  private void ExecuteFireballAttack(Vector3 targetPosition)
  {
    if (_boss.FireballSpawnPoint == null) return;

    Vector3 spawnPos = _boss.FireballSpawnPoint.position;

    // 1. Fireball Central
    Vector3 centerDir = (targetPosition - spawnPos).normalized;
    FireSingleBall(spawnPos, centerDir, _boss.FireballSpeed);

    // 2. Fireballs Laterales
    Vector3 flatDirectionToTarget = (new Vector3(targetPosition.x, spawnPos.y, targetPosition.z) - spawnPos).normalized;

    // Calcular la distancia horizontal al objetivo
    float horizontalDistance = Vector3.Distance(new Vector3(spawnPos.x, 0, spawnPos.z), new Vector3(targetPosition.x, 0, targetPosition.z));

    // Angulo de flanqueo: más cerca, el ángulo debe ser más amplio para pasar cerca.
    float flankAngle = 20f; // Grados de desviación horizontal

    // Calcular la rotación inicial basada en la dirección central plana
    Quaternion baseRotation = Quaternion.LookRotation(flatDirectionToTarget);

    // Lateral Izquierdo: Rotar la dirección base a la izquierda (negativo)
    Quaternion leftRotation = baseRotation * Quaternion.Euler(0, -flankAngle, 0);
    Vector3 leftDir = leftRotation * Vector3.forward;

    // Ajustar el pitch (inclinación vertical) para que apunte hacia la altura del jugador.
    // Esto asegura que la bola no pase por encima si el jugador está cerca.
    leftDir = CalculateFlankingPitch(spawnPos, leftDir, targetPosition);
    FireSingleBall(spawnPos, leftDir, _boss.FireballSpeed);

    // Lateral Derecho: Rotar la dirección base a la derecha (positivo)
    Quaternion rightRotation = baseRotation * Quaternion.Euler(0, flankAngle, 0);
    Vector3 rightDir = rightRotation * Vector3.forward;
    rightDir = CalculateFlankingPitch(spawnPos, rightDir, targetPosition);
    FireSingleBall(spawnPos, rightDir, _boss.FireballSpeed);
  }


  // Método auxiliar para el ajuste de pitch de las bolas laterales
  private Vector3 CalculateFlankingPitch(Vector3 spawnPos, Vector3 flatFlankDirection, Vector3 targetPos)
  {
    // Proyectar el punto de impacto deseado a cierta distancia frente al jugador.
    // Esto ayuda a que las bolas laterales pasen "cerca" del jugador y no por encima o debajo.
    float targetDistance = Vector3.Distance(spawnPos, targetPos);

    // El punto de impacto vertical (Y) debería estar cerca de la altura del jugador.
    // Creamos un objetivo artificial a la misma altura que el jugador, pero en la dirección de flanqueo.
    Vector3 artificialTarget = spawnPos + (flatFlankDirection * targetDistance);
    artificialTarget.y = targetPos.y + 0.5f; // Ajuste de altura a la cabeza del jugador (ej. 0.5m)

    // Calcular la dirección final (incluyendo el pitch)
    return (artificialTarget - spawnPos).normalized;
  }

  private void FireSingleBall(Vector3 position, Vector3 direction, float speed)
  {
    GameObject fireballGO = FireballPoolManager.Instance.GetFireball();

    fireballGO.transform.position = position;
    fireballGO.transform.rotation = Quaternion.LookRotation(direction);

    Rigidbody rb = fireballGO.GetComponent<Rigidbody>();
    if (rb != null)
    {
      rb.linearVelocity = direction * speed;
    }
    // Si no usa Rigidbody, el script Fireball.cs necesitaría actualizarse para 
    // manejar el movimiento en su Update/FixedUpdate usando la dirección y velocidad.
    // Asumiendo que usa Rigidbody por simplicidad.
  }
}