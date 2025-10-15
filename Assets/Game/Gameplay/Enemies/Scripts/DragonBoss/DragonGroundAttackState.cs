using UnityEngine;

public class DragonGroundAttackState : IState
{
  private DragonBossController _boss;
  private DragonStateFactory _factory;
  private string _attackAnimation;
  private bool _attackExecuted = false;

  // Simulación de un Collider de Daño (para el Aliento de Fuego o Mordida)
  private Collider _damageTrigger;

  public DragonGroundAttackState(DragonBossController boss, DragonStateFactory factory, string animName)
  {
    _boss = boss;
    _factory = factory;
    _attackAnimation = animName;
    _damageTrigger = animName == "Drakaris" ? _boss.FlameCollider : _boss.BiteCollider;
  }

  public void OnEnter()
  {
    _boss.Animator.SetTrigger(_attackAnimation);
    _boss.Rb.linearVelocity = Vector3.zero;
    _attackExecuted = false;

    // Nota: El collider de daño real debe ser activado/desactivado con Animation Events
  }

  public void Tick()
  {
    // Esperar a que la animación termine
    AnimatorStateInfo stateInfo = _boss.Animator.GetCurrentAnimatorStateInfo(0);

    // Lógica de Rotación: Seguir al objetivo LENTAMENTE durante el ataque (para no fallar)
    Transform target = _boss.CurrentTarget;
    if (target != null)
    {
      Vector3 flatDirection = (new Vector3(target.position.x, _boss.transform.position.y, target.position.z) - _boss.transform.position).normalized;
      Quaternion lookRotation = Quaternion.LookRotation(flatDirection);
      _boss.transform.rotation = Quaternion.Slerp(_boss.transform.rotation, lookRotation, Time.deltaTime * 1f);
    }
    if (!_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.3f)
    {
      Debug.Log($"ATAQUE EJECUTADO: {_attackAnimation} en {stateInfo.normalizedTime * 100f}%");
      _damageTrigger.enabled = true;
      if (_attackAnimation == "Drakaris") _boss.flame.SetActive(true);
      _attackExecuted = true;
    }
    if (_attackExecuted && stateInfo.IsName(_attackAnimation) && stateInfo.normalizedTime >= 0.7f)
    {
      // Si la animación termina, regresa a Reposo para elegir la siguiente acción.
      _boss.ChangeState(_factory.GroundIdle());
    }
  }

  public void OnExit()
  {
    if (_damageTrigger != null) _damageTrigger.enabled = false;
    if (_attackAnimation == "Drakaris") _boss.flame.SetActive(false);
  }

  // Este método sería llamado desde DragonBossController.OnAnimationEvent(string eventName)
  public void HandleAttackEvent(string eventName)
  {
    // Ejemplo de uso de Animation Event:
    // if (eventName == "ActivateDamage") _damageTrigger.enabled = true;
    // if (eventName == "DeactivateDamage") _damageTrigger.enabled = false;
  }
}