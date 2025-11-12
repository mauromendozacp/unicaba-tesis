using UnityEngine;

public class MiniDragonGroundIdleState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private float _timer;
  private float _minIdle = 0.5f;
  private float _maxIdle = 1.0f;

  public MiniDragonGroundIdleState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetBool("Idle", true);
    _timer = Random.Range(_minIdle, _maxIdle);
    _boss.StopMovement();

    // Elegir objetivo al entrar
    _boss.FindNearestPlayer();
    //Debug.Log(_boss.CurrentTarget);

    // Probabilidad de gritar al entrar en Idle
    /*if (Random.value < 0.2f)
    {
      _boss.Animator.SetTrigger("Scream");
    }*/
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;
    if (_timer > 0) return;
    //Debug.Log(_boss.CurrentTarget);
    // 1. Prioridad: Buscar y atacar
    if (_boss.CurrentTarget != null)
    {
      float dist = _boss.DistanceToTarget();
      bool canMelee = (Time.time - _boss.lastMeleeTime) >= _boss.MeleeCooldown;
      bool canRange = (Time.time - _boss.lastRangedTime) >= _boss.RangedCooldown;

      if (dist <= _boss.MinMeleeDistance && canMelee)
      {
        // Ataque cuerpo a cuerpo aleatorio (Mordisco o Cola)
        string attack = Random.value < 0.5f ? "BasicAttack" : "TailAttack";
        _boss.ChangeState(_factory.MeleeAttack(attack));
      }
      else if (dist <= _boss.ChaseRadius && canRange)
      {
        // Ataque a rango (Fireball)
        _boss.ChangeState(_factory.RangedAttack());
      }
      else
      {
        // 2. Transiciones de Movimiento/Vuelo
        int action = Random.Range(0, 10);
        //int action = 0;
        if (action < 6) // 60% Correr
        {
          _boss.ChangeState(_factory.GroundMove());
        }
        else if (action < 9) // 30% Reposicionamiento Aéreo
        {
          _boss.ChangeState(_factory.TransitionTakeoff());
        }
        else // 10% Reposo extendido
        {
          _timer = Random.Range(_minIdle, _maxIdle) * 2f;
        }
      }
    }
    else
    {
      // Si no hay target, intenta volar y buscar
      _boss.ChangeState(_factory.TransitionTakeoff());
    }
  }

  public void OnExit()
  {
    _boss.Animator.SetBool("Idle", false);
  }
}