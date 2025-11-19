using UnityEngine;
using System.Collections;

public class SkeletonEnemy : EnemySoldier
{
  SkeletonAnimationController animator;
  public SkeletonAnimationController Animator => animator;
  private float originalChaseRadius;
  private float originalMoveSpeed;

  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new SkeletonIdleState(this));

    if (selfCollider != null) selfCollider.enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Start()
  {
    base.Start();
    animator = GetComponent<SkeletonAnimationController>();
    SetAttackCollider(false);

    originalChaseRadius = ChaseRadius;
    originalMoveSpeed = moveSpeed;
  }

  public override void TakeDamage(float damage)
  {
    if (!IsAlive) return;

    if (currentState.State == EnemyState.Idle && IsTerritorial)
    {
      Debug.Log("¡SkeletonEnemy ha entrado en FURIA!");
      float newMoveSpeed = originalMoveSpeed * 1.35f;
      SetSpeed(newMoveSpeed);
      isTerritorial = false;
      SetChaseRadius(originalChaseRadius * 3f);
    }

    base.TakeDamage(damage);
    StartCoroutine(DamageMaterial());
    if (!IsAlive)
    {
      Kill();
    }
  }

  private IEnumerator DamageMaterial()
  {
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(0.1f);
    ToggleDamageMaterial(false);
    //yield break;
  }


  public override void Kill()
  {
    ChangeState(new SkeletonDeathState(this));
    //ToggleDamageMaterial(false);
  }

  public void EnableAttackCollider()
  {
    SetAttackCollider(true);
  }

  public void DisableAttackCollider()
  {
    SetAttackCollider(false);
  }
}