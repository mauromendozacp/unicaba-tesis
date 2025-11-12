using UnityEngine;

public class MiniDragonAirRangedAttackState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private bool _attackExecuted = false;
  private float _attackDuration = 1.5f;
  private float _timer;

  public MiniDragonAirRangedAttackState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("FlyFireballShoot");
    _boss.Rb.linearVelocity = Vector3.zero; // Fija el dragón durante el disparo
    _boss.CurrentAttack = MiniDragonAttackType.FIREBALL_AIR;
    _boss.lastRangedTime = Time.time; // Inicia Cooldown para rango (aplica a ambos)
    _attackExecuted = false;
    _timer = _attackDuration;
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;
    _boss.LookAtTarget(); // Asegura que el Dragón esté mirando al objetivo

    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);

    // Disparar las bolas de fuego justo después del inicio de la animación
    if (!_attackExecuted && stateInfo.IsName("Fly Fireball Shoot") && stateInfo.normalizedTime >= 0.2f)
    {
      FireMultipleBalls(_boss.FireballCount);
      _attackExecuted = true;
    }

    // Si el tiempo de ataque se acaba o la animación termina, transiciona
    if (_timer <= 0 || (stateInfo.IsName("Fly Fireball Shoot") && stateInfo.normalizedTime >= 0.7f))
    {
      // Después del ataque, reposicionarse o aterrizar
      if (Random.value > 0.5f)
      {
        _boss.ChangeState(_factory.TransitionLanding());
      }
      else
      {
        _boss.ChangeState(_factory.AirReposition());
      }
    }
  }

  public void OnExit()
  {
    _boss.CurrentAttack = MiniDragonAttackType.NONE;
  }

  // Método de disparo (reutilizado de RangedAttackState, pero sin abanico)
  private void FireMultipleBalls(int count)
  {
    Vector3 spawnPos = _boss.FireballSpawnPoint.position;
    Vector3 targetPos = _boss.CurrentTarget.position;

    // Disparo recto (oblicuo) hacia el objetivo, sin gravedad
    Vector3 launchDirection = (targetPos - spawnPos).normalized;

    for (int i = 0; i < count; i++)
    {
      // Simular una ligera dispersión para los ataques múltiples
      float spread = _boss.CurrentStance == CombatStance.ENRAGED ? 5f : 10f;
      float randomX = Random.Range(-spread, spread);
      float randomY = Random.Range(-spread, spread);

      Quaternion spreadRotation = Quaternion.Euler(randomX, randomY, 0);
      Vector3 finalDirection = spreadRotation * launchDirection;

      _boss.FireSingleBall(spawnPos, finalDirection);
    }
  }
}