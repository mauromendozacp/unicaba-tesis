using UnityEngine;
using System.Collections;

public class MiniDragonDieState : IState
{
  private MiniDragonController _boss;
  private MiniDragonStateFactory _factory;

  public MiniDragonDieState(MiniDragonController boss, MiniDragonStateFactory factory)
  {
    _boss = boss;
    _factory = factory;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger("Die");
    _boss.EnableCollisions();
    _boss.Rb.useGravity = true;
    _boss.StopMovement();

    //_boss.Rb.isKinematic = false;
    //_boss.Rb.linearVelocity = Vector3.zero;
    //_boss.Rb.isKinematic = true;
    //_boss.Rb.useGravity = true; // Asegura que caiga si estaba volando

    // Desactivar movimiento y colisiones para que quede inmóvil
    //_boss.DisableMovementAndCollisions();

    // Llamar al método de limpieza final del controlador
    _boss.StartCoroutine(DieCorroutine());
  }

  public void Tick()
  {
    // No hay lógica activa; solo espera la animación.
  }

  public void OnExit() { }

  private IEnumerator DieCorroutine()
  {
    yield return new WaitForSeconds(3f);
    _boss.Die();
  }
}