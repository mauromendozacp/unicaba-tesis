using UnityEngine;

public class DragonGroundAttackState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private string _attackAnimation;
  private bool _attackExecuted = false;
  float _attackRotationSpeed;
  bool _isFireballAttack = false;

  private const float ALIGNMENT_THRESHOLD = 3.0f;

  private Collider _damageTrigger;

  public DragonGroundAttackState(DragonBossController boss, DragonStateFactory factory, string animName)
  {
    _boss = boss;
    _factory = factory;
    _attackRotationSpeed = _boss.AttackRotationSpeed;

    if (animName == "Drakaris" || animName == "Bite")
    {
      _damageTrigger = animName == "Drakaris" ? _boss.FlameCollider : _boss.BiteCollider;
      _boss.CurrentAttack = animName == "Drakaris" ? DragonAttackType.GROUND_FLAME : DragonAttackType.GROUND_BITE;
      _isFireballAttack = false;
      _attackAnimation = animName;
    }
    else if (animName == "FireBall")
    {
      _isFireballAttack = true;
      _attackAnimation = "Drakaris";
      _boss.CurrentAttack = DragonAttackType.GROUND_FIREBALL;
      _damageTrigger = null;
    }
  }

  public void OnEnter()
  {
    _boss.IsVulnerable = true;
    _boss.Animator.SetTrigger(_attackAnimation);
    //_boss.Rb.linearVelocity = Vector3.zero;
    _boss.StopMovement();
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

    float angleToTarget = Vector3.Angle(_boss.transform.forward, flatDirection);

    if (!_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.3f)
    {
      if (_isFireballAttack)
      {
        ExecuteFireballAttack(target.position);
      }
      else
      {
        _damageTrigger.enabled = true;
        if (_attackAnimation == "Drakaris") _boss.flame.SetActive(true);
      }
      _attackExecuted = true;
    }
    if (_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.7f)
    {
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    if (_damageTrigger != null && !_isFireballAttack) _damageTrigger.enabled = false;
    if (_attackAnimation == "Drakaris") _boss.flame.SetActive(false);
    _boss.CurrentAttack = DragonAttackType.GROUND_NONE;
    //_boss.Rb.linearVelocity = Vector3.zero;
    _boss.StopMovement();
  }

  /*
    // Este método sería llamado desde DragonBossController.OnAnimationEvent(string eventName)
    public void HandleAttackEvent(string eventName)
    {
      // Ejemplo de uso de Animation Event:
      // if (eventName == "ActivateDamage") _damageTrigger.enabled = true;
      // if (eventName == "DeactivateDamage") _damageTrigger.enabled = false;
    }
    */

  private void ExecuteFireballAttack(Vector3 targetPosition)
  {
    if (_boss.FireballSpawnPoint == null)
    {
      Debug.LogWarning("FireballSpawnPoint no asignado en DragonBossController.");
      return;
    }

    Vector3 spawnPos = _boss.FireballSpawnPoint.position;

    // 1. Fireball Central
    Vector3 centerDir = (targetPosition - spawnPos).normalized;
    FireSingleBall(spawnPos, centerDir, _boss.FireballSpeed);

    // 2. Fireballs Laterales
    Vector3 flatDirectionToTarget = (new Vector3(targetPosition.x, spawnPos.y, targetPosition.z) - spawnPos).normalized;
    float flankAngle = 20f; // Grados de desviación horizontal
    Quaternion baseRotation = Quaternion.LookRotation(flatDirectionToTarget);

    // Lateral Izquierdo: Rotar la dirección base a la izquierda (negativo)
    Vector3 leftDir = baseRotation * Quaternion.Euler(0, -flankAngle, 0) * Vector3.forward;
    leftDir = CalculateFlankingPitch(spawnPos, leftDir, targetPosition);
    FireSingleBall(spawnPos, leftDir, _boss.FireballSpeed);

    // Lateral Derecho: Rotar la dirección base a la derecha (positivo)
    Vector3 rightDir = baseRotation * Quaternion.Euler(0, flankAngle, 0) * Vector3.forward;
    rightDir = CalculateFlankingPitch(spawnPos, rightDir, targetPosition);
    FireSingleBall(spawnPos, rightDir, _boss.FireballSpeed);
  }


  // Método auxiliar para las bolas laterales
  // Proyectar el punto de impacto deseado a cierta distancia frente al jugador.
  // Esto ayuda a que las bolas laterales pasen "cerca" del jugador y no por encima o debajo.
  private Vector3 CalculateFlankingPitch(Vector3 spawnPos, Vector3 flatFlankDirection, Vector3 targetPos)
  {
    float targetDistance = Vector3.Distance(spawnPos, targetPos);

    // Creamos un objetivo artificial a la misma altura que el jugador, pero en la dirección de flanqueo.
    Vector3 artificialTarget = spawnPos + (flatFlankDirection * targetDistance);
    artificialTarget.y = targetPos.y + 0.5f; // Ajuste de altura a la cabeza del jugador
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
  }
}