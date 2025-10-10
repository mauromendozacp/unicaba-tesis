using UnityEngine;

public class SpiderAnimationController : MonoBehaviour
{
  readonly string animWalk = "Walk";
  readonly string animAttack = "Attack";
  readonly string animIdle = "Idle";
  readonly string animDeath = "Death";
  readonly string animJump = "Jump";
  private Animator anim = null;

  private void Awake()
  {
    anim = GetComponent<Animator>();
  }

  public void ToggleWalk(bool walk)
  {
    anim?.SetBool(animWalk, walk);
  }

  public void ToggleIdle(bool walk)
  {
    anim?.SetBool(animIdle, walk);
  }

  public void TriggerAttack()
  {
    anim?.SetTrigger(animAttack);
  }

  public void TriggerDamage()
  {
    anim?.SetTrigger(animJump);
  }

  public void TriggerDeath()
  {
    anim?.SetTrigger(animDeath);
  }

}
