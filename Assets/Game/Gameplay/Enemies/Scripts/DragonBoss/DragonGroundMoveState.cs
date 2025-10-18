using UnityEngine;

public class DragonGroundMoveState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private float _minAttackDistance = 5f;
  private float _maxAttackDistance = 10f;

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
    /*
    Vector3 currentPos = _boss.transform.position;

    Vector3 flatDirection = (new Vector3(targetPos.x, currentPos.y, targetPos.z) - currentPos).normalized;
    float distance = Vector3.Distance(currentPos, targetPos);

    if (flatDirection.sqrMagnitude > 0.01f)
    {
      Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
      _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed);
    }
    */
    float distance = _boss.DistanceToTarget();
    if (distance < _minAttackDistance)
    {
      //_boss.Rb.linearVelocity = Vector3.zero;
      _boss.StopMovement();
      _boss.ChangeState(_factory.GroundAttack("Bite"));
    }
    else if (distance > _maxAttackDistance)
    {
      //_boss.Rb.linearVelocity = flatDirection * _boss.groundMoveSpeed;
      _boss.MoveTo(targetPos);
    }
    else
    {
      //_boss.Rb.linearVelocity = Vector3.zero;
      _boss.StopMovement();
      _boss.ChangeState(_factory.GroundAttack("Drakaris"));
    }
    // si agent.updateRotation es falso, llamar a LookAtTarget para rotar hacia el objetivo
    _boss.LookAtTarget();
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("Walk", false);
    //_boss.Rb.linearVelocity = Vector3.zero;
    _boss.StopMovement();
  }
}