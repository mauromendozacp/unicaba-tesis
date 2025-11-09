using UnityEngine;

public class MiniDragonGroundMoveState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private float _minAttackDistance = 3f; // Distancia para ataque cuerpo a cuerpo
  private float _maxRangedDistance = 15f; // Distancia para ataque a rango

  public MiniDragonGroundMoveState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
    _minAttackDistance = _boss.MinMeleeDistance;
  }

  public void OnEnter()
  {
    _boss.Animator.SetBool("Run", true);
    _boss.Agent.speed = _boss.Agent.enabled ? _boss.groundMoveSpeed : 0f;
    _boss.Agent.isStopped = false;
  }

  public void Tick()
  {
    Transform target = _boss.CurrentTarget;
    if (target == null)
    {
      _boss.ChangeState(_factory.GroundIdle());
      return;
    }

    float distance = _boss.DistanceToTarget();
    bool canMelee = (Time.time - _boss.lastMeleeTime) >= _boss.MeleeCooldown;
    bool canRange = (Time.time - _boss.lastRangedTime) >= _boss.RangedCooldown;

    // 1. Prioridad de Ataque y Distancia
    if (distance <= _minAttackDistance && canMelee)
    {
      _boss.StopMovement();
      string attack = Random.value < 0.5f ? "BasicAttack" : "TailAttack";
      _boss.ChangeState(_factory.MeleeAttack(attack));
    }
    else if (distance <= _maxRangedDistance && canRange)
    {
      // En rango de ataque a distancia y Cooldown listo
      _boss.StopMovement();
      _boss.ChangeState(_factory.RangedAttack());
    }
    else
    {
      // 2. Persecución

      // La IA puede optar por alejarse si está demasiado cerca para un rango (ej. entre 3m y 8m)
      if (distance < _maxRangedDistance && canRange)
      {
        Vector3 fleeDirection = (_boss.transform.position - target.position).normalized;
        Vector3 retreatPos = _boss.transform.position + fleeDirection * 5f;
        _boss.MoveTo(retreatPos);
      }
      else
      {
        _boss.MoveTo(target.position);
      }

      //_boss.Animator.speed = _boss.Agent.velocity.magnitude > 0.1f ? _boss.Agent.velocity.magnitude / _boss.Agent.speed : 1f;
    }

    // 3. Opción de Vuelo (Escape o Reposicionamiento)
    if (Random.value < 0.1f * Time.deltaTime) // Baja probabilidad de volar durante la persecución
    {
      _boss.ChangeState(_factory.TransitionTakeoff());
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("Run", false);
    _boss.StopMovement();
    _boss.Animator.speed = 1f;
  }
}