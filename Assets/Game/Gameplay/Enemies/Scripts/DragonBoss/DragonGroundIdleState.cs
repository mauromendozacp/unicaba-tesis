using UnityEngine;

public class DragonGroundIdleState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private float _timer;
  private float _minIdle = 0.75f;
  private float _maxIdle = 1.5f;

  public DragonGroundIdleState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.IsVulnerable = true;
    _boss.Animator.SetBool("IdleSimple", true);
    _timer = Random.Range(_minIdle, _maxIdle);
    //_boss.Rb.linearVelocity = Vector3.zero;
    //_boss.Rb.useGravity = true;
    _boss.FindNearestPlayer();
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;

    if (_timer <= 0)
    {
      if (_boss.CurrentTarget == null)
      {
        _boss.ChangeState(_factory.TransitionTakeoff());
        return;
      }

      int action = Random.Range(0, 4);
      if (action <= 1)
        _boss.ChangeState(_factory.GroundMove());
      else if (action == 2)
        _boss.ChangeState(_factory.TransitionTakeoff());
      else if (action == 3)
        _boss.ChangeState(_factory.GroundAttack("FireBall"));
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("IdleSimple", false);
  }
}
