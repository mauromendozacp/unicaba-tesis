using UnityEngine;

public class DragonBabyRangedAnimator : MonoBehaviour
{
  readonly string animWalk = "Walk";
  readonly string animRun = "Run";
  readonly string animRetreat = "Retreat";
  readonly string animAttack = "FireballShoot";
  readonly string animIdle = "Idle";
  readonly string animDeath = "Die";
  readonly string animHurt = "Hurt";

  private Animator anim = null;

  private void Awake()
  {
    anim = GetComponent<Animator>();
  }

  public void ToggleWalk(bool walk)
  {
    anim?.SetBool(animWalk, walk);
  }

  public void ToggleRun(bool run)
  {
    anim?.SetBool(animRun, run);
  }

  public void ToggleRetreat(bool retreat)
  {
    anim?.SetBool(animRetreat, retreat);
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
    anim?.SetTrigger(animHurt);
  }

  public void TriggerDeath()
  {
    anim?.SetTrigger(animDeath);
  }
}