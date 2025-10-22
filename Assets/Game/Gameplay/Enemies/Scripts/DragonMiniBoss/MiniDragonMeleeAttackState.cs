using UnityEngine;

public class MiniDragonMeleeAttackState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private string _attackAnimation;
  bool _attackExecuted;

  public MiniDragonMeleeAttackState(MiniDragonController boss, MiniDragonStateFactory factory, string animName)
  {
    _boss = boss;
    _factory = factory;
    _attackAnimation = animName;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger(_attackAnimation);
    _boss.StopMovement();
    _boss.lastMeleeTime = Time.time;
    _attackExecuted = false;

    // Configurar tipo de ataque y daño
    _boss.CurrentAttack = (_attackAnimation == "BasicAttack") ? MiniDragonAttackType.BITE : MiniDragonAttackType.TAIL;
  }

  public void Tick()
  {
    _boss.LookAtTarget();
    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);

    if (!_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.3f)
    {
      if (_attackAnimation == "BasicAttack") _boss.biteAttack.ToggleCollider(true);
      else _boss.tailAttack.ToggleCollider(true);
      _attackExecuted = true;
    }

    // Salir después de la recuperación
    if (_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.7f)
    {
      if (_attackAnimation == "BasicAttack") _boss.biteAttack.ToggleCollider(false);
      else _boss.tailAttack.ToggleCollider(false);
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    _boss.CurrentAttack = MiniDragonAttackType.NONE;
  }
}