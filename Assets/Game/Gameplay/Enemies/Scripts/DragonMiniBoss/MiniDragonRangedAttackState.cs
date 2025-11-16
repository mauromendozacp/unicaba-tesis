using UnityEngine;

public class MiniDragonRangedAttackState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private bool _attackExecuted = false;

  public MiniDragonRangedAttackState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("FireballShoot");
    _boss.StopMovement();
    _boss.lastRangedTime = Time.time;
    _boss.CurrentAttack = MiniDragonAttackType.FIREBALL_GROUND;
    _attackExecuted = false;
  }

  public void Tick()
  {
    _boss.LookAtTarget();

    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);
    if (!_attackExecuted && stateInfo.IsName("Fireball Shoot") && stateInfo.normalizedTime >= 0.3f)
    {
      FireMultipleBalls(_boss.FireballCount);
      _attackExecuted = true;
    }

    // Salir después de la recuperación
    if (stateInfo.IsName("Fireball Shoot") && stateInfo.normalizedTime >= 0.7f)
    {
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    _boss.CurrentAttack = MiniDragonAttackType.NONE;
  }

  private void FireMultipleBalls(int count)
  {
    // Implementación simple de disparo en abanico (o recto si count=1)
    Vector3 spawnPos = _boss.FireballSpawnPoint.position;
    Vector3 targetPos = _boss.CurrentTarget.position;

    float angleStep = (count > 1) ? 20f / (count - 1) : 0;
    float startAngle = (count > 1) ? -10f : 0;

    Quaternion baseRotation = Quaternion.LookRotation(targetPos - spawnPos);

    if (_boss.fireballSound != null) GameManager.Instance.AudioManager.PlayAudio(_boss.fireballSound);

    for (int i = 0; i < count; i++)
    {
      float angle = startAngle + i * angleStep;
      Quaternion spreadRotation = Quaternion.Euler(0, angle, 0);

      Vector3 direction = spreadRotation * (targetPos - spawnPos).normalized;

      _boss.FireSingleBall(spawnPos, direction);
    }
  }
}