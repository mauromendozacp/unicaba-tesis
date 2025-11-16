
using UnityEngine;

public class DragonBabyRangedAttackState : IEnemyState
{
  private readonly DragonBabyRanged enemy;
  public EnemyState State { get; private set; }

  public DragonBabyRangedAttackState(DragonBabyRanged enemy)
  {
    this.enemy = enemy;
    State = EnemyState.Attack;
  }

  public void Enter()
  {
    enemy.StopMovement();
    enemy.Animator.TriggerAttack();
  }

  public void Update()
  {
    if (enemy.CurrentTarget == null || !enemy.isTargetAlive())
    {
      enemy.ChangeState(new DragonBabyRangedIdleState(enemy));
      return;
    }

    float distanceToTarget = enemy.DistanceToTarget();

    enemy.LookAtTarget();

    // 1. Sale si el objetivo se sale del rango de ataque (óptimo +/- margen)
    if (distanceToTarget > enemy.OptimalAttackDistance + enemy.AttackRangeMargin ||
        distanceToTarget < enemy.OptimalAttackDistance - enemy.AttackRangeMargin)
    {
      enemy.ChangeState(new DragonBabyRangedChaseState(enemy));
      return;
    }

    // 2. Ejecutar ataque si el cooldown lo permite
    if (Time.time >= enemy.LastAttackTime + enemy.AttackCooldown)
    {
      if (enemy.fireballSound != null)
      {
        GameManager.Instance.AudioManager.PlayAudio(enemy.fireballSound);
      }
      enemy.ShootFireBall();
      // Después de disparar, retrocede ligeramente para evitar ser predecible
      enemy.ChangeState(new DragonBabyRangedRetreatState(enemy));
    }
  }

  public void Exit()
  {
  }
}