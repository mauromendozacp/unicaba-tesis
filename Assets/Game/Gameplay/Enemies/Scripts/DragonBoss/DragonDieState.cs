using UnityEngine;

public class DragonDieState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;

  public DragonDieState(DragonBossController boss, DragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    //_boss.animator.Play("die");
    _boss.Animator.SetTrigger("Die");
    _boss.Rb.linearVelocity = Vector3.zero;
    _boss.Rb.useGravity = true;

    // Desactivar el control de la IA
    _boss.enabled = false;

    Debug.Log("¡El Dragón ha sido derrotado!");
  }

  public void Tick()
  {
    // Se podría añadir lógica de fade-out o explosión aquí
  }

  public void OnExit() { } // Nunca se sale del estado de muerte
}