
using UnityEngine;

public class DragonBabyRangedAttackState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }
  bool _attackExecuted = false;

  public DragonBabyRangedAttackState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.StopMovement();
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
      return;
    }
    enemy.Animator.TriggerAttack();

  }

  public void Update()
  {
    enemy.LookAtTarget();

    if (!_attackExecuted && enemy.Animator.HasAttackAnimationPassedPercentage(0.6f))
    {
      if (enemy.fireballSound != null)
      {
        GameManager.Instance.AudioManager.PlayAudio(enemy.fireballSound);
      }
      enemy.ShootFireBall();
      _attackExecuted = true;
    }
    if (_attackExecuted && enemy.Animator.HasAttackAnimationPassedPercentage(0.75f))
    {
      enemy.ChangeState(new DragonBabyRangedRetreatState(enemy));
    }
  }

  public void Exit() { }
}