using UnityEngine;

public class DragonTransitionLandingState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;

  public DragonTransitionLandingState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    //_boss.animator.Play("lands");
    _boss.Animator.SetTrigger("Lands");
    _boss.Rb.useGravity = true; // Reactivar gravedad (cae hacia el suelo)
  }

  public void Tick()
  {
    // Esperar a que el Dragón toque el suelo si la animación es corta, 
    // o esperar a que termine la animación.

    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);
    if (stateInfo.IsName("Lands") && stateInfo.normalizedTime >= 0.75f)
    {
      // Opcional: Generar una onda expansiva de daño al aterrizar aquí.

      // Transición a estado de reposo en tierra
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    _boss.Rb.linearVelocity = Vector3.zero;
  }
}