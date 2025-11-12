using UnityEngine;

public class DragonAirHoverAttackState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private float _attackDuration = 4.0f;
  private float _timer;
  private float _bombDropInterval = 0.5f;
  private float _bombTimer;


  public DragonAirHoverAttackState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.IsVulnerable = false;
    _boss.Animator.SetBool("Hover", true);
    _boss.Rb.linearVelocity = Vector3.zero;
    _boss.Rb.useGravity = false;
    _boss.CurrentAttack = DragonAttackType.AIR_FIREBALL;
    _timer = _attackDuration;
    _bombTimer = _bombDropInterval;
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;
    _bombTimer -= Time.deltaTime;

    // Mantener la mirada en el objetivo mientras bombardea
    Transform target = _boss.CurrentTarget;
    if (target != null)
    {
      Vector3 targetFlatPos = new Vector3(target.position.x, _boss.transform.position.y, target.position.z);
      Vector3 flatDirection = (targetFlatPos - _boss.transform.position).normalized;
      Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
      _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed / 5);
    }

    if (_bombTimer <= 0)
    {
      if (target == null)
      {
        _boss.ChangeState(_factory.AirFlyReposition());
        return;
      }
      ExecuteAirBombAttack(target);
      _bombTimer = _bombDropInterval;
    }

    if (_timer <= 0)
    {
      // Termina el bombardeo y decide si aterriza o se reposiciona
      if (Random.value > 0.5f)
      {
        _boss.ChangeState(_factory.TransitionLanding());
      }
      else
      {
        _boss.ChangeState(_factory.AirFlyReposition());
      }
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("Hover", false);
    _boss.CurrentAttack = DragonAttackType.AIR_NONE;
  }

  private void ExecuteAirBombAttack(Transform target)
  {
    if (_boss.FireballSpawnPoint == null)
    {
      Debug.LogWarning("FireballSpawnPoint no asignado en DragonBossController.");
      return;
    }

    Vector3 spawnPos = _boss.FireballSpawnPoint.position;
    Vector3 targetPos = target.position;

    Vector3 launchDirection = (targetPos - spawnPos).normalized;

     _boss.FireSingleBall(spawnPos, launchDirection);
  }
}