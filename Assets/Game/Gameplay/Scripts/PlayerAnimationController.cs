using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim = null;

    private readonly string moveXKey = "MoveX";
    private readonly string moveYKey = "MoveY";
    private readonly string isDeadKey = "IsDead";
    private readonly string victoryKey = "Victory";

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateMoveAnimation(Vector3 move)
    {
        anim.SetFloat(moveXKey, move.x);
        anim.SetFloat(moveYKey, move.z);
    }

    public void ToggleDead(bool dead)
    {
        anim.SetBool(isDeadKey, dead);
    }

    public void TriggerVictory()
    {
        anim.SetTrigger(victoryKey);
    }
}
