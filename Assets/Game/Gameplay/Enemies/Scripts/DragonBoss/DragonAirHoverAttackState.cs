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
    //_boss.animator.Play("hover");
    _boss.Animator.SetBool("Hover", true);
    _boss.Rb.linearVelocity = Vector3.zero; // Se queda flotando en su lugar
    _boss.Rb.useGravity = false;
    _timer = _attackDuration;
    _bombTimer = _bombDropInterval;
  }

  public void Tick()
  {
    _timer -= Time.deltaTime;
    _bombTimer -= Time.deltaTime;

    // Mantener la mirada en el objetivo mientras bombardea
    //Transform target = _boss.GetTarget();
    Transform target = _boss.CurrentTarget;
    if (target != null)
    {
      Vector3 targetFlatPos = new Vector3(target.position.x, _boss.transform.position.y, target.position.z);
      Vector3 flatDirection = (targetFlatPos - _boss.transform.position).normalized;
      Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
      _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * _boss.rotationSpeed);
    }

    if (_bombTimer <= 0)
    {
      // Lógica de "Bombardeo":
      // Aquí llamarías a una función para instanciar una bola de fuego que caiga
      Debug.Log("DRAGÓN BOMBARDEANDO al objetivo.");
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
  }
}