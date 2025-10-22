using UnityEngine;

public class MiniDragonInitialTravelState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;
  private float _arrivalThreshold = 1.0f;

  public MiniDragonInitialTravelState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.EnableMovementAndCollisions(); // Activa NavMeshAgent
    //_boss.Animator.SetBool("Run", true);
    _boss.Animator.SetBool("Walk", true);

    // Mover a la posición inicial predefinida
    _boss.MoveTo(_boss.InitialDestination);
  }

  public void Tick()
  {
    // Usa la velocidad actual del agente para la animación
    //float agentSpeed = _boss.Agent.velocity.magnitude;
    //_boss.Animator.speed = agentSpeed > 0.1f ? agentSpeed / _boss.Agent.speed : 1f;

    // Comprobar si ha llegado a la posición
    if (_boss.Agent.remainingDistance <= _arrivalThreshold && _boss.Agent.velocity.sqrMagnitude < 0.1f)
    {
      _boss.StopMovement();
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    //_boss.Animator.SetBool("Run", false);
    _boss.Animator.SetBool("Walk", false);
    _boss.Animator.speed = 1f;
  }
}